using System.Diagnostics.CodeAnalysis;

namespace Toyana.Identity.Models;

public class VendorUserRole
{
    [SetsRequiredMembers]
    public VendorUserRole(Guid vendorUserId, Guid vendorRoleId)
    {
        VendorUserId = vendorUserId;
        VendorRoleId = vendorRoleId;
    }

    public required Guid       VendorUserId { get; set; }
    public virtual VendorUser VendorUser   { get; set; } = null!;

    public required Guid       VendorRoleId { get; set; }
    public virtual VendorRole VendorRole   { get; set; } = null!; // EF Core will set this
}