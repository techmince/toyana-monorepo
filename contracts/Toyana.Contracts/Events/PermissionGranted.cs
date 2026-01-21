namespace Toyana.Contracts.Events;

public record PermissionGranted(Guid UserId, Guid VendorId, string PermissionAction);
