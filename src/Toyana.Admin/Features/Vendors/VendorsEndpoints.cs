using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Toyana.VendorCenter.Data;
using Wolverine.Http;

namespace Toyana.Admin.Features.Vendors;

public static class VendorsEndpoints
{
    [WolverineGet("/admin/vendors")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> ListVendors(VendorDbContext db)
    {
        return Results.Ok(await db.Vendors.ToListAsync());
    }
}