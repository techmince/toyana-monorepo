using Toyana.Contracts;

namespace Toyana.Payments.Aggregates;

public class Payment
{
    public Guid              Id           { get; set; } // Same as OrderId for correlation
    public decimal           TotalAmount  { get; set; }
    public decimal           NetAmount    { get; set; } // After Ingress Fee
    public decimal           PlatformFees { get; set; }
    public string            Status       { get; set; }
    public List<PaymentItem> Splits       { get; set; } = new();

    // Event Sourcing Methods (Marten Convention: Apply)
    public void Apply(PaymentStarted @event)
    {
        Id          = @event.PaymentId; // Using OrderId as PaymentId usually
        TotalAmount = @event.TotalAmount;
        Status      = "Started";
    }

    public void Apply(PaymentCaptured @event)
    {
        Status       =  "Captured";
        PlatformFees += @event.IngressFee;
        NetAmount    =  @event.NetAmount;
    }

    public void Apply(VendorPayoutScheduled @event)
    {
        PlatformFees += @event.EgressFee;
        // Logic to track payouts per vendor
    }
}