namespace Toyana.Contracts.Events;

public record SubUserCreated(Guid SubUserId, Guid OwnerId, Guid VendorId);
