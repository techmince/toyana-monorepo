using System.Text.Json.Serialization;

namespace Toyana.Identity.Models;

public class VendorUserPermission
{
    public required Guid PermissionId { get; set; }
    public required Guid VendorUserId { get; set; }
    [JsonIgnore] public VendorUser VendorUser { get; set; } = null!;
}