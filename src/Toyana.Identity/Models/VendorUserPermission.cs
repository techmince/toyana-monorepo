using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Toyana.Identity.Models;

public class VendorUserPermission
{
    [SetsRequiredMembers]
    public VendorUserPermission(Guid permissionId, Guid vendorUserId)
    {
        PermissionId = permissionId;
        VendorUserId = vendorUserId;
    }
    public required     Guid       PermissionId { get; set; }
    public required     Guid       VendorUserId { get; set; }
    [JsonIgnore] public VendorUser VendorUser   { get; set; } = null!;
}