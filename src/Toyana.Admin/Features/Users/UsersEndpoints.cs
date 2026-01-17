using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toyana.Identity.Data;
using Wolverine;
using Wolverine.Http;
using Toyana.Contracts;

namespace Toyana.Admin.Features.Users;

public static class UsersEndpoints
{
    [WolverineGet("/admin/users")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> ListUsers(ApplicationDbContext db)
    {
        var clients = await db.ClientUsers.Select(u => new { Type="Client", u.Id, u.Username, u.CreatedAt }).ToListAsync();
        var vendors = await db.VendorUsers.Select(u => new { Type="Vendor", u.Id, u.Username, CreatedAt=DateTime.MinValue }).ToListAsync(); 
        return Results.Ok(clients.Cast<object>().Concat(vendors));
    }

    [WolverinePost("/admin/users/{id}/ban")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> BanUser(
        Guid id, 
        ApplicationDbContext db, 
        IMessageBus bus)
    {
        var client = await db.ClientUsers.FindAsync(id);
        if (client != null) 
        { 
            client.IsBanned = true; 
            await db.SaveChangesAsync(); 
            await bus.PublishAsync(new Contracts.UserBanned(id, "Banned by Admin"));
            return Results.Ok("User Banned"); 
        }
        
        var vendor = await db.VendorUsers.FindAsync(id);
        if (vendor != null) 
        { 
            vendor.IsBanned = true; 
            await db.SaveChangesAsync(); 
            await bus.PublishAsync(new Contracts.UserBanned(id, "Banned by Admin"));
            return Results.Ok("Vendor Banned"); 
        }
        
        return Results.NotFound();
    }

    [WolverinePost("/admin/users/{id}/password")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> ResetPassword(
        Guid id, 
        [FromBody] string newPassword, 
        ApplicationDbContext db)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        var client = await db.ClientUsers.FindAsync(id);
        if (client != null) { client.PasswordHash = hash; await db.SaveChangesAsync(); return Results.Ok("Password Reset"); }
        
        var vendor = await db.VendorUsers.FindAsync(id);
        if (vendor != null) { vendor.PasswordHash = hash; await db.SaveChangesAsync(); return Results.Ok("Password Reset"); }

        return Results.NotFound();
    }
}
