using System;

namespace Toyana.Contracts;

// Commands
public record ProcessPayment(Guid OrderId, decimal TotalAmount, List<PaymentItem> Items, string Currency = "USD");
public record PaymentItem(Guid VendorId, decimal Amount);

// Events
public record PaymentStarted(Guid PaymentId, Guid OrderId, decimal TotalAmount, string Currency);
public record PaymentCaptured(Guid PaymentId, decimal Amount, decimal IngressFee, decimal NetAmount);
public record VendorPayoutScheduled(Guid PaymentId, Guid VendorId, decimal GrossAmount, decimal EgressFee, decimal PayoutAmount);
public record PayoutReleased(Guid PaymentId, Guid VendorId, decimal Amount);
public record PaymentFailed(Guid PaymentId, string Reason);
