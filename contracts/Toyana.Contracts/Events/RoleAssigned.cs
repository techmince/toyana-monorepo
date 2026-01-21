namespace Toyana.Contracts.Events;

public record RoleAssigned(Guid UserId, Guid VendorId, string RoleName);
