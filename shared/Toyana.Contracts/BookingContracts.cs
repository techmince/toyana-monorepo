namespace Toyana.Contracts;

// Commands
public record RequestBooking(Guid BookingId, Guid VendorId, Guid UserId, DateTime EventDate, decimal Amount);
public record ApproveBooking(Guid BookingId, Guid VendorId);
public record RejectBooking(Guid BookingId, Guid VendorId, string Reason);
public record RecordPaymentAuthorization(Guid BookingId, string PaymentTransactionId);
public record ConfirmServiceDelivery(Guid BookingId);

// Events
public record BookingRequested(Guid BookingId, Guid VendorId, Guid UserId, DateTime EventDate, decimal Amount);
public record BookingApproved(Guid BookingId, DateTime Timestamp);
public record BookingConfirmed(Guid BookingId, DateTime Timestamp);
public record BookingRejected(Guid BookingId, string Reason, DateTime Timestamp);
public record PaymentAuthorized(Guid BookingId, string PaymentTransactionId, DateTime Timestamp);
public record ServiceDelivered(Guid BookingId, DateTime Timestamp);
public record BookingPayoutReleased(Guid BookingId, decimal Amount, DateTime Timestamp);
public record BookingPriceAdjusted(Guid BookingId, decimal NewAmount, string Reason, DateTime Timestamp);
