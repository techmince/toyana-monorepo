namespace Toyana.Contracts;

public record UserCreated(Guid UserId, string Username, string PhoneNumber);
public record UserBanned(Guid UserId, string Reason);
public record UserUnbanned(Guid UserId, string Reason);
public record VendorUserCreated(Guid UserId, Guid VendorId, string Username, bool IsOwner);
