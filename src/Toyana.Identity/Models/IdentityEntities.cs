using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Toyana.Identity.Models;

public class ClientUser : AbstractUser
{
    public Guid Id { get; set; }
    [MaxLength(200)] public required string Username { get; set; }
    [MaxLength(200)] public required string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsBanned { get; set; }
}

public class VendorUser : AbstractUser
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }

    [MaxLength(200)] public required string Username { get; set; }
    public bool IsOwner { get; set; }
    public bool IsBanned { get; set; }

    public List<VendorUserRole> Roles { get; set; } = new();
    public List<VendorUserPermission> Permissions { get; set; } = new();
}

public class VendorRole
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    [MaxLength(200)] public required string Name { get; set; }
    public List<string> Permissions { get; set; } = new();
}

// Join Table for VendorUser <-> VendorRole
public class VendorUserRole
{
    public Guid VendorUserId { get; set; }
    public VendorUser VendorUser { get; set; } = null!;

    public Guid VendorRoleId { get; set; }
    public VendorRole VendorRole { get; set; } = null!; // EF Core will set this
}

// Direct Permissions assignment
public class VendorUserPermission
{
    public Guid Id { get; set; }
    public Guid VendorUserId { get; set; }

    [JsonIgnore] public VendorUser VendorUser { get; set; } = null!;

    [MaxLength(200)] public required string Permission { get; set; }
}

public interface IUser
{
    [MaxLength(200)] public string PasswordHash { get; }
    [MaxLength(200)] public string Salt { get; }
    bool VerifyPassword(string pass);
}

public abstract class AbstractUser : IUser
{
    [MaxLength(200)] public string PasswordHash { get; set; }
    [MaxLength(200)] public string Salt { get; set; }
    public bool VerifyPassword(string pass) => BCrypt.Net.BCrypt.Verify(pass + Salt, PasswordHash);
}

public class AdminUser : AbstractUser
{
    public Guid Id { get; set; }
    [MaxLength(300)] public required string Username { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}