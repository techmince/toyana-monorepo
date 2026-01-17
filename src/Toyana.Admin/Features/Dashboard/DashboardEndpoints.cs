using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Toyana.Identity.Data;
using Toyana.Ordering.Features.Bookings;
using Toyana.VendorCenter.Data;
using Wolverine.Http;

namespace Toyana.Admin.Features.Dashboard;

public static class DashboardEndpoints
{
    [WolverineGet("/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> GetDashboard(
        ApplicationDbContext identityDb, 
        VendorDbContext vendorDb, 
        IQuerySession orderSession)
    {
        var clientCount = await EntityFrameworkQueryableExtensions.CountAsync(identityDb.ClientUsers);
        var vendorCount = await EntityFrameworkQueryableExtensions.CountAsync(identityDb.VendorUsers);
        var serviceCount = await EntityFrameworkQueryableExtensions.CountAsync(vendorDb.Services);
        var bookingCount = await QueryableExtensions.CountAsync(orderSession.Query<Booking>());

        return Results.Ok(new 
        {
            Clients = clientCount,
            Vendors = vendorCount,
            Services = serviceCount,
            Bookings = bookingCount
        });
    }
}
