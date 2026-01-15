using Microsoft.EntityFrameworkCore;
using Toyana.Identity.Data;
using Wolverine;
using Microsoft.AspNetCore.Mvc;

namespace Toyana.Admin.Features.Users;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");

        // List Users
        group.MapGet("/", async (ApplicationDbContext db) => 
        {
            var clients = await db.ClientUsers.Select(u => new { Type="Client", u.Id, u.Username, u.CreatedAt }).ToListAsync();
            var vendors = await db.VendorUsers.Select(u => new { Type="Vendor", u.Id, u.Username, CreatedAt=DateTime.MinValue }).ToListAsync(); 
            return Results.Ok(clients.Cast<object>().Concat(vendors));
        });

        // Ban User
        group.MapPost("/{id}/ban", async (Guid id, ApplicationDbContext db, IMessageBus bus) => 
        {
            var client = await db.ClientUsers.FindAsync(id);
            if (client != null) 
            { 
                client.IsBanned = true; 
                await db.SaveChangesAsync(); 
                await bus.PublishAsync(new Toyana.Contracts.UserBanned(id, "Banned by Admin"));
                return Results.Ok("User Banned"); 
            }
            
            var vendor = await db.VendorUsers.FindAsync(id);
            if (vendor != null) 
            { 
                vendor.IsBanned = true; 
                await db.SaveChangesAsync(); 
                await bus.PublishAsync(new Toyana.Contracts.UserBanned(id, "Banned by Admin"));
                return Results.Ok("Vendor Banned"); 
            }
            
            return Results.NotFound();
        });

        // Reset Password
        group.MapPost("/{id}/password", async (Guid id, [FromBody] string newPassword, ApplicationDbContext db) => 
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var client = await db.ClientUsers.FindAsync(id);
            if (client != null) { client.PasswordHash = hash; await db.SaveChangesAsync(); return Results.Ok("Password Reset"); }
            
            var vendor = await db.VendorUsers.FindAsync(id);
            if (vendor != null) { vendor.PasswordHash = hash; await db.SaveChangesAsync(); return Results.Ok("Password Reset"); }

            return Results.NotFound();
        });
    }
}
