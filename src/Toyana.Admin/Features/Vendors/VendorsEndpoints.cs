using Microsoft.EntityFrameworkCore;
using Toyana.VendorCenter.Data;

namespace Toyana.Admin.Features.Vendors;

public static class VendorsEndpoints
{
    public static void MapVendorsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/vendors", async (VendorDbContext db) =>
        {
            return Results.Ok(await db.Vendors.ToListAsync());
        });
    }
}
