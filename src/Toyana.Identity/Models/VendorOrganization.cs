using System.ComponentModel.DataAnnotations;
using Toyana.Contracts.Exceptions;
using Toyana.Shared.Entity;

namespace Toyana.Identity.Models;

public class VendorOrganization : Entity
{
    [MaxLength(200)] public required string BusinessName { get; set; }
    [MaxLength(100)] public required string TaxId        { get; set; }
    [MaxLength(100)] public required string Category     { get; set; }

    public required Guid OwnerId { get; set; } // Enforces single owner
    public VendorUser Owner { get; set; } = null!;

    public bool      IsDeleted  { get; set; } = false; // Soft delete
    public DateTime? DeletedAt  { get; set; }

    public virtual List<VendorUser> Users { get; set; } = new();

    // Domain Methods

    /// <summary>
    ///     Soft delete organization (owner only)
    /// </summary>
    public void SoftDelete(VendorUser requestingUser)
    {
        if (requestingUser.Id != OwnerId)
            throw new DomainException("FORBIDDEN", "Only the owner can delete the organization");

        if (IsDeleted)
            throw new DomainException("ALREADY_DELETED", "Organization is already deleted");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Restore organization (owner only)
    /// </summary>
    public void Restore(VendorUser requestingUser)
    {
        if (requestingUser.Id != OwnerId)
            throw new DomainException("FORBIDDEN", "Only the owner can restore the organization");

        if (!IsDeleted)
            throw new DomainException("NOT_DELETED", "Organization is not deleted");

        IsDeleted = false;
        DeletedAt = null;
    }
}
