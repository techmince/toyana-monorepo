using Toyana.Payments.Interfaces;

namespace Toyana.Payments.Services;

public class FeeCalculator(IFeeStrategy strategy)
{
    public decimal CalculateIngressFee(decimal totalAmount)
    {
        return strategy.CalculateIngressFee(totalAmount);
    }

    public decimal CalculateEgressFee(decimal vendorAmount)
    {
        return strategy.CalculateEgressFee(vendorAmount);
    }
}