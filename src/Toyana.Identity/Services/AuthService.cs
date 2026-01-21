using Microsoft.EntityFrameworkCore;
using Toyana.Contracts;
using Toyana.Contracts.Exceptions;
using Toyana.Identity.Data;
using Toyana.Identity.Domain.Repositories;
using Toyana.Identity.Models;
using Wolverine;

namespace Toyana.Identity.Services;

public class AuthService
{
    private readonly IMessageBus          _bus;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AuthService> _logger;
    private readonly ITokenService        _tokenService;

    public AuthService
    (
        ApplicationDbContext db,
        IMessageBus bus,
        ILogger<AuthService> logger,
        ITokenService tokenService
    )
    {
        _db           = db;
        _bus          = bus;
        _logger       = logger;
        _tokenService = tokenService;
    }

    // --- CLIENT FLOW ---
    public async Task<AuthResponse> RegisterClientAsync(RegisterRequest request)
    {
        if (await _db.ClientUsers.AnyAsync(u => u.Username    == request.Username ||
                                                u.PhoneNumber == request.PhoneNumber))
        {
            _logger.LogWarning("Registration failed: User {Username} or {PhoneNumber} already exists",
                               request.Username,
                               request.PhoneNumber);
            throw new DomainException("USER_EXISTS", "User already exists.");
        }

        var user = new ClientUser(Guid.NewGuid(), request.Username, request.PhoneNumber, request.Password);

        _db.ClientUsers.Add(user);
        await _db.SaveChangesAsync();

        await _bus.PublishAsync(new UserCreated(user.Id, user.Username, user.PhoneNumber));

        _logger.LogInformation("Client User {UserId} registered successfully", user.Id);

        var token = await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Customer");
        return token;
    }

    public async Task<AuthResponse> LoginClientAsync(LoginRequest request)
    {
        var user = await _db.ClientUsers.SingleOrDefaultAsync(u =>
                                                                  u.Username    == request.Login ||
                                                                  u.PhoneNumber == request.Login);

        if (user is null)
        {
            _logger.LogWarning("Login failed for {Login}: User not found", request.Login);
            throw new DomainException("NOT_FOUND", "User not found");
        }

        if (user.IsBanned)
        {
            _logger.LogWarning("Login blocked for {Login}: User is banned", request.Login);
            throw new DomainException("BANNED", "User is banned.");
        }

        if (!user.VerifyPassword(request.Password))
        {
            _logger.LogWarning("Login failed for {Login}: Invalid credentials", request.Login);
            throw new DomainException("INVALID_CREDENTIALS", "Invalid credentials.");
        }

        _logger.LogInformation("Client User {UserId} logged in", user.Id);
        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Customer");
    }

    // --- VENDOR FLOW ---
    public async Task<AuthResponse> RegisterVendorOwnerAsync(VendorRegisterRequest request)
    {
        if (await _db.VendorUsers.AnyAsync(u => u.Username == request.Username))
        {
            _logger.LogWarning("Vendor Registration failed: User {Username} already exists",
                               request.Username);
            throw new DomainException("USER_EXISTS", "Username already exists");
        }

        var ownerId = Guid.NewGuid();
        var orgId   = Guid.NewGuid();

        // Create organization first
        var organization = new VendorOrganization
                           {
                               Id           = orgId,
                               BusinessName = request.BusinessName,
                               TaxId        = request.TaxId,
                               Category     = request.Category,
                               OwnerId      = ownerId
                           };

        // Create owner user
        var owner = new VendorUser(ownerId, orgId, request.Username, request.Password, VendorRoleType.Owner);

        _db.VendorOrganizations.Add(organization);
        _db.VendorUsers.Add(owner);
        await _db.SaveChangesAsync();

        await _bus.PublishAsync(new VendorCreated(orgId, request.BusinessName, request.TaxId, request.Category));

        _logger.LogInformation("VendorOrganization {OrgId} created with Owner {OwnerId}", orgId, ownerId);

        return await _tokenService.GenerateTokenAsync(owner.Id.ToString(), "Vendor", orgId, true, new List<string>());
    }

    public async Task<AuthResponse> CreateSubUserAsync(Guid requestingUserId, CreateSubUserRequest request)
    {
        var requestingUser = await _db.VendorUsers
                                      .Include(u => u.VendorOrganization)
                                      .FirstOrDefaultAsync(u => u.Id == requestingUserId);

        if (requestingUser == null)
            throw new DomainException("NOT_FOUND", "User not found");

        // Only Owner can create subusers
        if (requestingUser.Role != VendorRoleType.Owner)
            throw new DomainException("FORBIDDEN", "Only the owner can create subusers");

        if (await _db.VendorUsers.AnyAsync(u => u.Username == request.Username))
            throw new DomainException("USER_EXISTS", "Username already exists");

        // Validate role
        if (!Enum.IsDefined(typeof(VendorRoleType), request.Role))
            throw new DomainException("INVALID_ROLE", "Invalid role specified");

        if (request.Role == VendorRoleType.Owner)
            throw new DomainException("FORBIDDEN", "Cannot create another owner");

        // Use domain factory method
        var subUser = requestingUser.CreateSubUser(request.Username, request.Password, request.Role);

        _db.VendorUsers.Add(subUser);
        await _db.SaveChangesAsync();

        _logger.LogInformation("SubUser {UserId} created for Organization {OrgId} with Role {Role}",
                               subUser.Id, requestingUser.VendorOrganizationId, request.Role);

        return new AuthResponse("CREATED", "", DateTime.UtcNow);
    }

    public async Task<AuthResponse> LoginVendorAsync(LoginRequest request)
    {
        var user = await _db.VendorUsers
                            .Include(u => u.VendorOrganization)
                            .SingleOrDefaultAsync(u => u.Username == request.Login);

        if (user is null)
        {
            _logger.LogWarning("Vendor Login failed for {Login}: User not found", request.Login);
            throw new DomainException("INVALID_CREDENTIALS", "Invalid credentials");
        }

        if (user.IsBanned)
        {
            _logger.LogWarning("Vendor Login blocked for {Login}: User is banned", request.Login);
            throw new DomainException("BANNED", "User is banned");
        }

        if (user.IsDeleted)
        {
            _logger.LogWarning("Vendor Login blocked for {Login}: User is deleted", request.Login);
            throw new DomainException("DELETED", "User is deleted");
        }

        if (!user.VerifyPassword(request.Password))
        {
            _logger.LogWarning("Vendor Login failed for {Login}: Invalid credentials", request.Login);
            throw new DomainException("INVALID_CREDENTIALS", "Invalid credentials");
        }

        // Get permissions based on role
        var permissions = new List<string>();
        switch (user.Role)
        {
            case VendorRoleType.Owner:
                permissions.Add("*"); // Owner has all permissions
                break;
            case VendorRoleType.Admin:
                permissions.AddRange(new[] { "MANAGE_SERVICES", "MANAGE_AVAILABILITY", "VIEW_FINANCIALS", "MANAGE_USERS", "DELETE_SUBUSER", "RESTORE_SUBUSER", "ACCEPT_BOOKING", "VIEW_BOOKINGS", "VIEW_CHAT" });
                break;
            case VendorRoleType.Manager:
                permissions.AddRange(new[] { "ACCEPT_BOOKING", "VIEW_BOOKINGS", "VIEW_CHAT" });
                break;
            case VendorRoleType.Staff:
                permissions.Add("VIEW_CHAT");
                break;
        }

        _logger.LogInformation("Vendor User {UserId} logged in for Organization {OrgId}", user.Id, user.VendorOrganizationId);
        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Vendor", user.VendorOrganizationId, user.IsOwner, permissions);
    }

    // --- ADMIN FLOW ---
    public async Task<AuthResponse> LoginAdminAsync(LoginRequest request)
    {
        var user = await _db.AdminUsers.SingleOrDefaultAsync(u => u.Username == request.Login);
        if (user == null || !user.VerifyPassword(request.Password)) // FIX: Add negation
        {
            _logger.LogWarning("Admin Login failed for {Login}", request.Login);
            throw new DomainException("INVALID_CREDENTIALS", "Invalid credentials.");
        }

        _logger.LogInformation("Admin User {UserId} logged in", user.Id);
        return await _tokenService.GenerateTokenAsync(user.Id.ToString(), "Admin");
    }
}