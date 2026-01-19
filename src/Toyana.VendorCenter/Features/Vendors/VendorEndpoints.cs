using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Http;
using Toyana.Contracts;
using Toyana.VendorCenter.Data;
using System.Security.Claims;
using Toyana.Shared;

namespace Toyana.VendorCenter.Features.Vendors;

public static class VendorEndpoints
{
    [WolverinePost("/vendors")]
    [Tags("Vendors")]
    public static async Task<IResult> CreateVendor(
        CreateVendor command, 
        IMessageBus bus)
    {
        await bus.InvokeAsync(command);
        return Results.Accepted();
    }

    [WolverinePost("/vendors/services")]
    [Authorize(Policy = "ManageServices")]
    [Tags("Vendors")]
    public static async Task<IResult> AddService(
        AddService command, 
        IMessageBus bus)
    {
        await bus.InvokeAsync(command);
        return Results.Accepted();
    }

    [WolverinePost("/vendors/availability")]
    [Authorize(Policy = "ManageAvailability")]
    [Tags("Vendors")]
    public static async Task<IResult> SetAvailability(
        SetAvailability command, 
        IMessageBus bus)
    {
        await bus.InvokeAsync(command);
        return Results.Accepted();
    }

    [WolverineGet("/vendors/services")]
    [Authorize]
    [Tags("Vendors")]
    public static async Task<IResult> GetServices(
        VendorDbContext db, 
        ClaimsPrincipal user)
    {
        var vendorId = user.GetVendorId();
        if (!vendorId.HasValue) return Results.Unauthorized();

        var services = await db.Services.Where(s => s.VendorId == vendorId.Value).ToListAsync();
        return Results.Ok(services);
    }

    [WolverineGet("/vendors/availability")]
    [Authorize]
    [Tags("Vendors")]
    public static async Task<IResult> GetAvailability(
        VendorDbContext db, 
        ClaimsPrincipal user)
    {
        var vendorId = user.GetVendorId();
        if (!vendorId.HasValue) return Results.Unauthorized();

        var slots = await db.AvailabilitySlots
            .Where(a => a.VendorId == vendorId.Value)
            .OrderBy(a => a.Date)
            .ToListAsync();
            
        return Results.Ok(slots);
    }
}
