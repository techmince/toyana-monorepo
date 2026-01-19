namespace Toyana.Payments.Interfaces;

public interface IDebitCardProvider
{
    // Real implementation would go to Stripe/PayPal
    Task<bool> ChargeAsync(decimal amount, string currency);
}

public interface IBankAccountProvider
{
    // Real implementation would payout via Bank/Wise/Stripe Connect
    Task<string> TransferAsync(decimal amount, Guid vendorId);
}
