using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Toyana.Identity.Models;

public class VendorUser : AbstractUser
{
    // new vendor user 
    [SetsRequiredMembers]
    public VendorUser(Guid id, Guid vendorId, string username, string password, bool isOwner = true)
    {
        Id = id;
        VendorId = vendorId;
        Username = username;
        IsOwner = true;
        IsBanned = false;
        Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, Salt);
    }

    // ef core ctor
    [SetsRequiredMembers]
    public VendorUser(Guid id, Guid vendorId, string username, bool isOwner, bool isBanned, string passwordHash,
        string salt)
    {
        Id = id;
        VendorId = vendorId;
        Username = username;
        IsOwner = isOwner;
        IsBanned = isBanned;
        PasswordHash = passwordHash;
        Salt = salt;
    }

    public required Guid VendorId { get; set; }

    [MaxLength(200)] public required string Username { get; set; }
    public required bool IsOwner { get; set; }
    public required bool IsBanned { get; set; }

    public virtual List<VendorUserRole> Roles { get; set; } = new();
    public virtual List<Permission> Permissions { get; set; } = new();
}