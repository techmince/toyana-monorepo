using System.Threading.Tasks;
using Npgsql;
using RestSharp;
using Xunit;

namespace Toyana.E2E.Tests;

public class ToyanaApiFixture : IAsyncLifetime
{
    public RestClient Client { get; private set; }
    
    // Connection Strings (pointing to localhost exposed ports)
    private const string IdentityDb = "Host=localhost;Port=5432;Database=toyana_identity;Username=postgres;Password=postgres;Include Error Detail=true";
    private const string VendorDb = "Host=localhost;Port=5432;Database=toyana_vendor;Username=postgres;Password=postgres;Include Error Detail=true";
    private const string CatalogDb = "Host=localhost;Port=5432;Database=toyana_catalog;Username=postgres;Password=postgres;Include Error Detail=true";
    private const string OrderingDb = "Host=localhost;Port=5432;Database=toyana_ordering;Username=postgres;Password=postgres;Include Error Detail=true";

    public Task InitializeAsync()
    {
        var options = new RestClientOptions("http://localhost:8080")
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        Client = new RestClient(options);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Cleanup Logic - Truncate tables to ensure clean state for next run
        // Notes: 
        // 1. We SKIP truncating "Users" in Identity to preserve the Admin user seeded by the app.
        // 2. We truncate Vendor/Ordering tables.
        
        await TruncateTable(VendorDb, "Vendors", "ServicePackages", "AvailabilityCalendar");
        
        // Catalog & Ordering use Marten, so tables are prefixed with mt_doc_ or mt_events
        // We will try running a rough cleanup. If tables don't exist yet (first run empty), catch exception.
        
        await TruncateTable(CatalogDb, "mt_doc_vendorreadmodel");
        await TruncateTable(OrderingDb, "mt_doc_booking", "wolverine_outbox", "wolverine_incoming"); 
        
        // Also clean wolverine message queues in DBs if possible? 
        // For simplicity, just business tables.
    }

    private async Task TruncateTable(string connectionString, params string[] tables)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            foreach (var table in tables)
            {
                // check if table exists first to avoid error? Or just try truncate.
                // Using CASCADE to handle FKs
                // Enclosing in double quotes to handle case sensitivity if Marten users lower case or whatever.
                // Marten usually uses lowercase table names.
                
                var cmdText = $"TRUNCATE TABLE \"{table.ToLower()}\" CASCADE"; 
                // Note: Marten tables are usually lowercase.
                
                // However, Npgsql might need checking. Docker postgres creates schema "public".
                // We'll wrap in try-catch to ignore if table doesn't exist (e.g. tests failed before creation).
                try 
                {
                    await using var cmd = new NpgsqlCommand(cmdText, conn);
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (PostgresException ex) when (ex.SqlState == "42P01") // Undefined table
                {
                    // Ignore
                }
            }
        }
        catch (Exception)
        {
            // Log or ignore global connection failure? 
            // In tests we might want to fail teardown to know, prevents dirty state.
            // But let's allow it for now.
        }
    }
}
