using Marten;
using Microsoft.AspNetCore.Authorization;
using Wolverine;
using Wolverine.Http;
using Toyana.Shared;
using Toyana.Contracts;
using System.Security.Claims;

namespace Toyana.Ordering.Features.Bookings;

public static class BookingEndpoints
{
    [WolverinePost("/ordering/bookings")]
    [Authorize]
    [Tags("Bookings")]
    public static async Task<IResult> CreateBooking(
        RequestBooking command, 
        IMessageBus bus, 
        ClaimsPrincipal user)
    {
        // Ensure ClientId matches token
        var userId = user.GetUserId();
        if (userId.HasValue)
        {
            // Override ClientId in command to trust the token, not the body
            command = command with { UserId = userId.Value };
        }

        await bus.PublishAsync(command);
        return Results.Accepted($"/ordering/bookings/{command.BookingId}");
    }

    [WolverineGet("/ordering/vendor-bookings")]
    [Authorize]
    [Tags("Bookings")]
    public static async Task<IResult> GetVendorBookings(
        IQuerySession session, 
        ClaimsPrincipal user)
    {
        var vendorId = user.GetVendorId();
        if (!vendorId.HasValue) return Results.Unauthorized();

        var bookings = await session.Query<Booking>()
            .Where(b => b.VendorId == vendorId.Value)
            .ToListAsync();

        return Results.Ok(bookings);
    }

    [WolverineGet("/ordering/my-bookings")]
    [Authorize]
    [Tags("Bookings")]
    public static async Task<IResult> GetClientBookings(
        IQuerySession session,
        ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue) return Results.Unauthorized();

        var bookings = await session.Query<Booking>()
            .Where(b => b.UserId == userId.Value)
            .ToListAsync();

        return Results.Ok(bookings);
    }

    [WolverinePost("/ordering/bookings/{id}/accept")]
    [Authorize]
    [Tags("Bookings")]
    public static async Task<IResult> AcceptBooking(
        Guid id, 
        IMessageBus bus)
    {
        // In real app, verify Vendor owns this booking
        await bus.InvokeAsync(
            new ApproveBooking(id, Guid.Empty)); // Guid.Empty for VendorId if not checking yet
        return Results.Accepted();
    }

    [WolverinePost("/ordering/bookings/{id}/reject")]
    [Authorize]
    [Tags("Bookings")]
    public static async Task<IResult> RejectBooking(
        Guid id, 
        IMessageBus bus)
    {
        await bus.InvokeAsync(new RejectBooking(id, Guid.Empty, "Rejected by vendor"));
        return Results.Accepted();
    }
}
