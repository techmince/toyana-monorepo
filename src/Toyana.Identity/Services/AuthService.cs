using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Toyana.Contracts;
using Toyana.Contracts.Exceptions;
using Toyana.Identity.Data;
using Toyana.Identity.Models;
using Wolverine;

namespace Toyana.Identity.Services;

public class AuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IMessageBus _bus;
    private readonly ILogger<AuthService> _logger;
    private readonly ITokenService _tokenService;

    public AuthService(ApplicationDbContext db, IMessageBus bus, ILogger<AuthService> logger, ITokenService tokenService)
    {
        _db = db;
        _bus = bus;
        _logger = logger;
        _tokenService = tokenService;
    }

    // --- CLIENT FLOW ---
    public async Task<AuthResponse> RegisterClientAsync(RegisterRequest request)
    {
        if (await _db.ClientUsers.AnyAsync(u => u.Username == request.Username || u.PhoneNumber == request.PhoneNumber))
        {
             _logger.LogWarning("Registration failed: User {Username} or {PhoneNumber} already exists.", request.Username, request.PhoneNumber);
             throw new DomainException("USER_EXISTS", "User already exists.");
        }

        var user = new ClientUser
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.ClientUsers.Add(user);
        await _db.SaveChangesAsync();

        await _bus.PublishAsync(new UserCreated(user.Id, user.Username, user.PhoneNumber));
        
        _logger.LogInformation("Client User {UserId} registered successfully.", user.Id);

        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Customer");
    }

    public async Task<AuthResponse> LoginClientAsync(LoginRequest request)
    {
        var user = await _db.ClientUsers.SingleOrDefaultAsync(u => u.Username == request.Login || u.PhoneNumber == request.Login);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
             _logger.LogWarning("Login failed for {Login}: Invalid credentials.", request.Login);
             throw new DomainException("INVALID_CREDENTIALS", "Invalid credentials.");
        }

        if (user.IsBanned)
        {
             _logger.LogWarning("Login blocked for {Login}: User is banned.", request.Login);
             throw new DomainException("BANNED", "User is banned.");
        }

        _logger.LogInformation("Client User {UserId} logged in.", user.Id);
        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Customer");
    }

    // --- VENDOR FLOW ---
    public async Task<AuthResponse> RegisterVendorOwnerAsync(VendorRegisterRequest request)
    {
        if (await _db.VendorUsers.AnyAsync(u => u.Username == request.Username))
        {
            _logger.LogWarning("Vendor Registration failed: User {Username} already exists.", request.Username);
            throw new DomainException("USER_EXISTS", "Vendor User already exists.");
        }

        var vendorId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new VendorUser
        {
            Id = userId,
            VendorId = vendorId, // Link to new Vendor
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsOwner = true
        };

        _db.VendorUsers.Add(user);
        await _db.SaveChangesAsync();

        await _bus.PublishAsync(new VendorCreated(vendorId, request.BusinessName, request.TaxId, request.Category)); 
        
        _logger.LogInformation("Vendor {VendorId} created by Owner {UserId}.", vendorId, userId);

        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Vendor", vendorId, isOwner: true, permissions: new List<string>());
    }

    public async Task<AuthResponse> CreateSubUserAsync(Guid ownerId, CreateSubUserRequest request)
    {
        var owner = await _db.VendorUsers.FindAsync(ownerId);
        if (owner == null) throw new DomainException("NOT_FOUND", "Owner not found.");
        // Double check owner status (Controller should have checked claim, but good to be safe)
        if (!owner.IsOwner) throw new DomainException("FORBIDDEN", "Only owners can create sub-users.");

        if (await _db.VendorUsers.AnyAsync(u => u.Username == request.Username))
             throw new DomainException("USER_EXISTS", "User already exists.");

        var newUser = new VendorUser
        {
            Id = Guid.NewGuid(),
            VendorId = owner.VendorId, // Same Vendor
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsOwner = false
        };
        
        // Add Permissions
        if (request.ExtraPermissions != null)
        {
            foreach(var p in request.ExtraPermissions)
            {
                newUser.Permissions.Add(new VendorUserPermission { Permission = p });
            }
        }

        // Add Roles (Simplified: assuming Role Names passed exist, or we create them? Plan said Role Tables exist. 
        // For MVP, let's skip dynamic Role lookup and just stick to Permissions or assume Roles are pre-seeded or passed by ID)
        // Providing ability to assign Role by Name if it exists for this vendor.
        if (request.Roles != null)
        {
            foreach(var roleName in request.Roles)
            {
                var role = await _db.VendorRoles.FirstOrDefaultAsync(r => r.VendorId == owner.VendorId && r.Name == roleName);
                if (role != null)
                {
                    newUser.Roles.Add(new VendorUserRole { VendorRole = role });
                }
            }
        }

        _db.VendorUsers.Add(newUser);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Sub-User {UserId} created for Vendor {VendorId} by Owner {OwnerId}.", newUser.Id, owner.VendorId, ownerId);

        return new AuthResponse("CREATED", "", DateTime.UtcNow); 
    }

    public async Task<AuthResponse> LoginVendorAsync(LoginRequest request)
    {
        var user = await _db.VendorUsers
            .Include(u => u.Roles).ThenInclude(r => r.VendorRole)
            .Include(u => u.Permissions)
            .SingleOrDefaultAsync(u => u.Username == request.Login);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Vendor Login failed for {Login}: Invalid credentials.", request.Login);
            throw new DomainException("INVALID_CREDENTIALS", "Invalid credentials.");
        }

        if (user.IsBanned)
        {
             _logger.LogWarning("Vendor Login blocked for {Login}: User is banned.", request.Login);
             throw new DomainException("BANNED", "User is banned.");
        }

        // Aggregate Permissions
        var permissions = new HashSet<string>();
        if (user.Roles != null)
        {
            foreach(var r in user.Roles)
            {
                if (r.VendorRole?.Permissions != null)
                    foreach(var p in r.VendorRole.Permissions) permissions.Add(p);
            }
        }
        if (user.Permissions != null)
            foreach(var p in user.Permissions) permissions.Add(p.Permission);

        _logger.LogInformation("Vendor User {UserId} logged in for Vendor {VendorId}.", user.Id, user.VendorId);
        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Vendor", user.VendorId, user.IsOwner, permissions.ToList());
    }

    // --- ADMIN FLOW ---
    public async Task<AuthResponse> LoginAdminAsync(LoginRequest request)
    {
        var user = await _db.AdminUsers.SingleOrDefaultAsync(u => u.Username == request.Login);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
             _logger.LogWarning("Admin Login failed for {Login}.", request.Login);
             throw new DomainException("INVALID_CREDENTIALS", "Invalid credentials.");
        }

        _logger.LogInformation("Admin User {UserId} logged in.", user.Id);
        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Admin");
    }
}
