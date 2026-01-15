using Toyana.Payments.Interfaces;

namespace Toyana.Payments.Services;

public class StandardFeeStrategy : IFeeStrategy
{
    private const decimal FeePercentage = 0.01m; // 1%

    public decimal CalculateIngressFee(decimal totalAmount)
    {
        return Math.Round(totalAmount * FeePercentage, 2);
    }

    public decimal CalculateEgressFee(decimal vendorAmount)
    {
        return Math.Round(vendorAmount * FeePercentage, 2);
    }
}
