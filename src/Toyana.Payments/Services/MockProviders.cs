using Toyana.Payments.Interfaces;

namespace Toyana.Payments.Services;

public class MockDebitCardProvider : IDebitCardProvider
{
    public Task<bool> ChargeAsync(decimal amount, string currency)
    {
        // Simulate Success
        return Task.FromResult(true);
    }
}

public class MockBankAccountProvider : IBankAccountProvider
{
    public Task<string> TransferAsync(decimal amount, Guid vendorId)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }
}