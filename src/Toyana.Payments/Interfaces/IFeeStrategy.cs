namespace Toyana.Payments.Interfaces;

public interface IFeeStrategy
{
    decimal CalculateIngressFee(decimal totalAmount);
    decimal CalculateEgressFee(decimal vendorAmount);
}
