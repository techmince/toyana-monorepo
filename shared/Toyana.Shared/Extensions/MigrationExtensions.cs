using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Toyana.Shared.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationAsync<TContext>(this IHost app) where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            // Using EnsureCreated for now as migration generation is blocked by .NET 10 tooling issues.
            // This ensures the schema exists.
            await context.Database.MigrateAsync();
            logger.LogInformation("Successfully ensured database creation for context {ContextName}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations for context {ContextName}", typeof(TContext).Name);
            throw;
        }
    }
}
