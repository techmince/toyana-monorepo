using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Toyana.Contracts.Exceptions;

namespace Toyana.Identity.Models;

public class VendorUser : AbstractUser
{
    // new vendor user
    [SetsRequiredMembers]
    public VendorUser(Guid id, Guid vendorOrganizationId, string username, string password, VendorRoleType role = VendorRoleType.Owner)
    {
        Id                     = id;
        VendorOrganizationId   = vendorOrganizationId;
        Username               = username;
        Role                   = role;
        IsBanned               = false;
        IsDeleted              = false;
        Salt                   = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        PasswordHash           = BCrypt.Net.BCrypt.HashPassword(password, Salt);
    }

    // ef core ctor
    [SetsRequiredMembers]
    public VendorUser
    (
        Guid id, Guid vendorOrganizationId, string username, VendorRoleType role, bool isBanned, bool isDeleted,
        string passwordHash, string salt
    )
    {
        Id                   = id;
        VendorOrganizationId = vendorOrganizationId;
        Username             = username;
        Role                 = role;
        IsBanned             = isBanned;
        IsDeleted            = isDeleted;
        PasswordHash         = passwordHash;
        Salt                 = salt;
    }

    public required Guid VendorOrganizationId { get; set; }
    public VendorOrganization VendorOrganization { get; set; } = null!;

    [MaxLength(200)] public required string Username { get; set; }

    public required VendorRoleType Role { get; set; }

    public required bool IsBanned  { get; set; }
    public required bool IsDeleted { get; set; }
    public DateTime?     DeletedAt { get; set; }

    // Computed property - user is owner if they are the organization's owner
    public bool IsOwner => VendorOrganization?.OwnerId == Id;

    // Domain Methods

    /// <summary>
    ///     Soft delete subuser (Admin or Owner only)
    /// </summary>
    public void SoftDelete(VendorUser requestingUser)
    {
        if (requestingUser.VendorOrganizationId != VendorOrganizationId)
            throw new DomainException("FORBIDDEN", "Cannot delete users from other organizations");

        if (IsOwner)
            throw new DomainException("FORBIDDEN", "Cannot delete the owner");

        if (requestingUser.Role != VendorRoleType.Owner && requestingUser.Role != VendorRoleType.Admin)
            throw new DomainException("FORBIDDEN", "Only Owner or Admin can delete subusers");

        if (IsDeleted)
            throw new DomainException("ALREADY_DELETED", "User is already deleted");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Restore subuser (Admin or Owner only)
    /// </summary>
    public void Restore(VendorUser requestingUser)
    {
        if (requestingUser.VendorOrganizationId != VendorOrganizationId)
            throw new DomainException("FORBIDDEN", "Cannot restore users from other organizations");

        if (requestingUser.Role != VendorRoleType.Owner && requestingUser.Role != VendorRoleType.Admin)
            throw new DomainException("FORBIDDEN", "Only Owner or Admin can restore subusers");

        if (!IsDeleted)
            throw new DomainException("NOT_DELETED", "User is not deleted");

        IsDeleted = false;
        DeletedAt = null;
    }

    /// <summary>
    ///     Check if this user can perform the specified permission
    /// </summary>
    public bool CanPerform(string permission)
    {
        return Role switch
        {
            VendorRoleType.Owner => true, // Owner can do everything
            VendorRoleType.Admin => permission != "DELETE_ORGANIZATION" && permission != "CREATE_SUBUSER",
            VendorRoleType.Manager => permission is "ACCEPT_BOOKING" or "VIEW_BOOKINGS" or "VIEW_CHAT",
            VendorRoleType.Staff => permission == "VIEW_CHAT",
            _ => false
        };
    }

    /// <summary>
    ///     Factory method: Creates a sub-user (owner only)
    /// </summary>
    public VendorUser CreateSubUser(string username, string password, VendorRoleType role)
    {
        if (Role != VendorRoleType.Owner)
            throw new DomainException("FORBIDDEN", "Only owners can create sub-users");

        if (role == VendorRoleType.Owner)
            throw new DomainException("FORBIDDEN", "Cannot create another owner");

        var subUser = new VendorUser(
                                     Guid.NewGuid(),
                                     VendorOrganizationId,
                                     username,
                                     password,
                                     role
                                    );

        return subUser;
    }
}