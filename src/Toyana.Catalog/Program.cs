using JasperFx;
using Marten;
using Toyana.Catalog.Models;
using Toyana.Shared.Extensions;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;
using Wolverine.RabbitMQ; // Observability

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.AddToyanaOpenApi();

// Observability
builder.AddToyanaObservability("catalog-api");
builder.AddToyanaJsonOptions();

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
builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseToyanaObservability();

if (app.Environment.IsDevelopment())
{

}

app.UseToyanaOpenApi();

app.MapGet("/", () => "Toyana.Catalog Service");

// Wolverine HTTP Endpoints
app.MapWolverineEndpoints(opts =>
{
});

app.Run();
