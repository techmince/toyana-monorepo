using JasperFx;
using Marten;
using Wolverine;
using Wolverine.Marten;
using Wolverine.RabbitMQ;
using Toyana.Catalog.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

using Toyana.Shared.Extensions; // Observability

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Observability
builder.AddToyanaObservability("catalog-api");

// Marten Configuration (Read Model Store)
builder.Services.AddMarten(opts =>
{
    var conn = builder.Configuration.GetConnectionString("Postgres");
    opts.Connection(conn);
    opts.DatabaseSchemaName = "catalog";
    opts.AutoCreateSchemaObjects = AutoCreate.All;
    
    // Indexing for search performance
    opts.Schema.For<VendorReadModel>().Index(x => x.Category);
})
.IntegrateWithWolverine();

// Wolverine Configuration
builder.Host.UseWolverine(opts =>
{
    var rabbitConn = builder.Configuration.GetConnectionString("RabbitMq");
    opts.UseRabbitMq(new Uri(rabbitConn))
        .AutoProvision();

    // Consume events from "toyana.catalog" queue which subscribes to Vendor events
    opts.PublishAllMessages().ToRabbitQueue("toyana.catalog");
    
    // In a real app, we'd be more specific about bindings. 
    // Here we rely on Wolverine's auto-discovery of handlers (VendorEventConsumers).
});

var app = builder.Build();

app.UseToyanaObservability();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Toyana.Catalog Service");

// Search API
app.MapGet("/catalog/search", async (IQuerySession session, string? category, string? date) =>
{
    // Basic query
    var query = session.Query<VendorReadModel>();

    if (!string.IsNullOrEmpty(category))
    {
        query = (Marten.Linq.IMartenQueryable<VendorReadModel>)query.Where(v => v.Category == category);
    }

    if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var dateOnly))
    {
        // Must NOT contain the date in BlockedDates
        // Marten's LINQ provider translates .Contains for arrays/lists in JSONB
        // Logic: Return vendors where BlockedDates does NOT contain requested date.
        query = (Marten.Linq.IMartenQueryable<VendorReadModel>)query.Where(v => !v.BlockedDates.Contains(dateOnly));
    }

    return await query.ToListAsync();
});

app.MapGet("/catalog/vendors/{id}", async (IQuerySession session, Guid id) =>
{
    var vendor = await session.LoadAsync<VendorReadModel>(id);
    return vendor is null ? Results.NotFound() : Results.Ok(vendor);
});

app.MapGet("/catalog/featured", async (IQuerySession session) =>
{
    // Mocking "Featured" by taking top 3. 
    // In future, sort by Rating if available.
    var vendors = await session.Query<VendorReadModel>()
        .Take(3)
        .ToListAsync();
    return Results.Ok(vendors);
});

app.Run();
