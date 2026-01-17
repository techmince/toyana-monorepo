using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toyana.Ordering.Features.Bookings;
using Wolverine;
using Wolverine.Http;
using Toyana.Contracts;

namespace Toyana.Admin.Features.Bookings;

public static class BookingsEndpoints
{
    [WolverineGet("/admin/bookings")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> ListBookings(IQuerySession session)
    {
        var bookings = await session.Query<Booking>().OrderByDescending(b => b.EventDate).ToListAsync();
        return Results.Ok(bookings);
    }

    [WolverinePost("/admin/bookings/{id}/reject")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static IResult RejectBooking(
        Guid id, 
        IMessageBus bus)
    {
        return Results.BadRequest("Not fully implemented: Saga needs Admin bypass");
    }

    [WolverinePost("/admin/bookings/{id}/price")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> AdjustPrice(
        Guid id, 
        [FromBody] decimal newAmount, 
        IDocumentSession session)
    {
        // Append event to stream to adjust price
        session.Events.Append(id, new Contracts.BookingPriceAdjusted(id, newAmount, "Admin Override", DateTime.UtcNow));
        await session.SaveChangesAsync();
        return Results.Ok("Price Adjusted");
    }

    [WolverineGet("/admin/bookings/{id}/trace")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> TraceEvents(
        Guid id, 
        IQuerySession session)
    {
        var events = await session.Events.FetchStreamAsync(id);
        return Results.Ok(events.Select(e => new { e.Id, Type = e.EventTypeName, e.Data, e.Timestamp }));
    }
}
