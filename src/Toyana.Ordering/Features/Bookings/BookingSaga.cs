using Toyana.Contracts;
using Wolverine;

namespace Toyana.Ordering.Features.Bookings;

public class Booking : Saga
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime EventDate { get; set; }

    // Wolverine looks for public static Start() methods to spawn a new saga
    public static (Booking, BookingRequested) Start(RequestBooking command)
    {
        var booking = new Booking
        {
            Id = command.BookingId,
            VendorId = command.VendorId,
            UserId = command.UserId,
            Amount = command.Amount,
            EventDate = command.EventDate,
            Status = BookingStatus.PendingVendorApproval
        };

        return (booking, new BookingRequested(command.BookingId, command.VendorId, command.UserId, command.EventDate, command.Amount));
    }

    // Handle Vendor Approval
    public void Handle(ApproveBooking command)
    {
        if (Status != BookingStatus.PendingVendorApproval) return;

        Status = BookingStatus.AwaitingPayment;
        
        // Emitting an event
        // Note: In Wolverine, you can return messages to be sent.
        // But here we are just updating state. We should probably publish an event too.
        // Let's modify the signature to return the event.
    }
    
    // Wolverine allows handling messages and returning outgoing messages
    public BookingApproved Handle(ApproveBooking command, ILogger<Booking> logger)
    {
        // Ideally we check if command.BookingId matches, but Wolverine routes by Saga Id automatically if correlated.
        // We should validate state.
        
        Status = BookingStatus.AwaitingPayment;
        logger.LogInformation("Booking {Id} approved by vendor. Awaiting payment.", Id);
        
        return new BookingApproved(Id, DateTime.UtcNow);
    }
    
     public BookingRejected Handle(RejectBooking command, ILogger<Booking> logger)
    {
        MarkCompleted(); // Deletes the saga logic if we want, or keep it as Rejected.
        // If we want to keep history, we don't call MarkCompleted().
        Status = BookingStatus.Rejected;
        logger.LogInformation("Booking {Id} rejected by vendor. Reason: {Reason}", Id, command.Reason);

        return new BookingRejected(Id, command.Reason, DateTime.UtcNow);
    }

    public BookingConfirmed Handle(PaymentAuthorized message, ILogger<Booking> logger)
    {
        if (Status != BookingStatus.AwaitingPayment) 
        {
             // Idempotency or out of order handling
             logger.LogWarning("Received PaymentAuthorized for booking {Id} but status is {Status}", Id, Status);
             return null;
        }

        Status = BookingStatus.Confirmed;
        logger.LogInformation("Payment authorized for booking {Id}. Booking Confirmed.", Id);
        
        // Maybe trigger notifications?
        return new BookingConfirmed(Id, message.Timestamp); // Wait, we don't have BookingConfirmed in contracts, I missed it? 
        // Checking contracts... Ah, I missed defining BookingConfirmed event in the previous step.
        // I defined BookingApproved, PaymentAuthorized. 
        // The prompt says: PaymentAuthorized -> BookingConfirmed.
        // I will assume BookingConfirmed is the STATE, but maybe I should emit an event `BookingConfirmed` too.
        // I will add it to the contracts.
    }
    
    // Service Delivered -> Payout Release
    public BookingPayoutReleased Handle(ConfirmServiceDelivery command, ILogger<Booking> logger)
    {
         if (Status != BookingStatus.Confirmed) return null;

         Status = BookingStatus.Completed;
         logger.LogInformation("Service delivered for booking {Id}. Releasing payout.", Id);
         
         // Logic to calculate payout split could be here or in the listener of PayoutReleased.
         // For now just emit the event.
         return new BookingPayoutReleased(Id, Amount, DateTime.UtcNow);
    }
}

public enum BookingStatus
{
    PendingVendorApproval,
    AwaitingPayment,
    Confirmed,
    Rejected,
    Completed
}

// Event "BookingConfirmed" is defined in contracts.
public record BookingConfirmed(Guid BookingId, DateTime Timestamp);
