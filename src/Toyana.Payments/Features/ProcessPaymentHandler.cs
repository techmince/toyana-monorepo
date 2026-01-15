using Marten;
using Wolverine;
using Toyana.Contracts;
using Toyana.Payments.Aggregates;
using Toyana.Payments.Interfaces;
using Toyana.Payments.Services;

namespace Toyana.Payments.Features;

public class ProcessPaymentHandler
{
    private readonly IDebitCardProvider _cardProvider;
    private readonly FeeCalculator _feeCalculator;

    public ProcessPaymentHandler(IDebitCardProvider cardProvider, FeeCalculator feeCalculator)
    {
        _cardProvider = cardProvider;
        _feeCalculator = feeCalculator;
    }

    public async Task Handle(ProcessPayment command, IDocumentSession session, IMessageBus bus)
    {
        var paymentStreamId = command.OrderId; // Correlation

        // 1. Start Payment
        session.Events.StartStream<Payment>(paymentStreamId, 
            new PaymentStarted(paymentStreamId, command.OrderId, command.TotalAmount, command.Currency));

        // 2. Calculate Fees & Charge
        var ingressFee = _feeCalculator.CalculateIngressFee(command.TotalAmount);
        var netAmount = command.TotalAmount - ingressFee;

        try 
        {
            // Call external provider
            var success = await _cardProvider.ChargeAsync(command.TotalAmount, command.Currency);
            
            if (success)
            {
                // 3. Record Capture
                session.Events.Append(paymentStreamId, new PaymentCaptured(paymentStreamId, command.TotalAmount, ingressFee, netAmount));

                // 4. Calculate Splits
                foreach (var item in command.Items)
                {
                    var egressFee = _feeCalculator.CalculateEgressFee(item.Amount);
                    var payoutAmount = item.Amount - egressFee;

                    session.Events.Append(paymentStreamId, 
                        new VendorPayoutScheduled(paymentStreamId, item.VendorId, item.Amount, egressFee, payoutAmount));
                    
                    // In a real system, we might schedule this for later (background job)
                    // For now, let's simulate immediate release awareness
                    session.Events.Append(paymentStreamId, new PayoutReleased(paymentStreamId, item.VendorId, payoutAmount));
                }
            }
            else
            {
                session.Events.Append(paymentStreamId, new PaymentFailed(paymentStreamId, "Provider Declined"));
            }
        }
        catch (Exception ex)
        {
            session.Events.Append(paymentStreamId, new PaymentFailed(paymentStreamId, ex.Message));
        }

        await session.SaveChangesAsync();
    }
}
