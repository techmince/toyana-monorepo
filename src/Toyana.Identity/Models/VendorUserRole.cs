namespace Toyana.Identity.Models;

public class VendorUserRole
{
    public Guid VendorUserId { get; set; }
    public VendorUser VendorUser { get; set; } = null!;

    public Guid VendorRoleId { get; set; }
    public VendorRole VendorRole { get; set; } = null!; // EF Core will set this
}