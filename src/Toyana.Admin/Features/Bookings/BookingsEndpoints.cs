using Marten;
using Wolverine;
using Toyana.Ordering.Features.Bookings;

namespace Toyana.Admin.Features.Bookings;

public static class BookingsEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/bookings");

        // List Bookings
        group.MapGet("/", async (IQuerySession session) =>
        {
            var bookings = await session.Query<Booking>().OrderByDescending(b => b.EventDate).ToListAsync();
            return Results.Ok(bookings);
        });

        // Reject Booking
        group.MapPost("/{id}/reject", async (Guid id, IMessageBus bus) => 
        {
            return Results.BadRequest("Not fully implemented: Saga needs Admin bypass");
        });

        // Adjust Price
        group.MapPost("/{id}/price", async (Guid id, decimal newAmount, IDocumentSession session) => 
        {
            // Append event to stream to adjust price
            session.Events.Append(id, new Toyana.Contracts.BookingPriceAdjusted(id, newAmount, "Admin Override", DateTime.UtcNow));
            await session.SaveChangesAsync();
            return Results.Ok("Price Adjusted");
        });

        // Trace Events
        group.MapGet("/{id}/trace", async (Guid id, IQuerySession session) => 
        {
            var events = await session.Events.FetchStreamAsync(id);
            return Results.Ok(events.Select(e => new { e.Id, Type = e.EventTypeName, e.Data, e.Timestamp }));
        });
    }
}
