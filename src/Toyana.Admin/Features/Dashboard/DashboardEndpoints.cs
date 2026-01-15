using Microsoft.EntityFrameworkCore;
using Marten;
using Toyana.Identity.Data;
using Toyana.VendorCenter.Data;
using Toyana.Ordering.Features.Bookings;

namespace Toyana.Admin.Features.Dashboard;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard", async (ApplicationDbContext identityDb, VendorDbContext vendorDb, IQuerySession orderSession) =>
        {
            var clientCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(identityDb.ClientUsers);
            var vendorCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(identityDb.VendorUsers);
            var serviceCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(vendorDb.Services);
            var bookingCount = await Marten.QueryableExtensions.CountAsync(orderSession.Query<Booking>());

            return Results.Ok(new 
            {
                Clients = clientCount,
                Vendors = vendorCount,
                Services = serviceCount,
                Bookings = bookingCount
            });
        });
    }
}
