using System.Text;
using JasperFx;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Wolverine;
using Wolverine.Marten;
using Wolverine.RabbitMQ;
using Marten;
using Toyana.Ordering.Features.Bookings;
using Toyana.Shared;
using Toyana.Shared.Extensions; // Observability

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Observability
builder.AddToyanaObservability("ordering-api");

// Auth (JWT) - Shared Key/Issuer with other services
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyForToyanaProjectAndItMustBeLongEnough";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Toyana",
            ValidAudience = "Toyana",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();


// Configure Marten (Document DB for Postgres)
builder.Services.AddMarten(opts =>
    {
        // In real app, get from config
        var connectionString = builder.Configuration.GetConnectionString("Postgres")
                               ??
                               "Host=localhost;Port=5432;Database=toyana_ordering;Username=postgres;Password=postgres";
        opts.Connection(connectionString);
        opts.DatabaseSchemaName = "ordering";
        opts.AutoCreateSchemaObjects = AutoCreate.All;

        // Index for querying by Vendor/Client
        opts.Schema.For<Booking>().Index(x => x.VendorId);
        opts.Schema.For<Booking>().Index(x => x.UserId);
    })
    .IntegrateWithWolverine(); // Connect Marten to Wolverine Outbox


// Configure Wolverine
builder.Host.UseWolverine(opts =>
{
    // Use RabbitMQ for external transport
    opts.UseRabbitMq(new Uri(builder.Configuration.GetConnectionString("RabbitMq") ??
                             "amqp://guest:guest@localhost:5672"));

    // Persist Sagas and Outbox using Marten (Postgres)
    opts.Policies.UseDurableLocalQueues();
    opts.Policies.AutoApplyTransactions();

    // Discovery of Sagas
    opts.Discovery.IncludeAssembly(typeof(Booking).Assembly);
});

var app = builder.Build();

app.UseToyanaObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Toyana.Ordering Service");

// Minimal API for Creating Booking
app.MapPost("/ordering/bookings",
    async (IMessageBus bus, Toyana.Contracts.RequestBooking command, System.Security.Claims.ClaimsPrincipal user) =>
    {
        // Ensure ClientId matches token
        var userId = user.GetUserId();
        if (userId.HasValue)
        {
            // Override ClientId in command to trust the token, not the body
            command = command with { UserId = userId.Value };
        }

        await bus.PublishAsync(command);
        return Results.Accepted($"/ordering/bookings/{command.BookingId}");
    }).RequireAuthorization();

// Vendor Kanban
app.MapGet("/ordering/vendor-bookings", async (IQuerySession session, System.Security.Claims.ClaimsPrincipal user) =>
{
    var vendorId = user.GetVendorId();
    if (!vendorId.HasValue) return Results.Unauthorized();

    var bookings = await session.Query<Booking>()
        .Where(b => b.VendorId == vendorId.Value)
        .ToListAsync();

    return Results.Ok(bookings);
}).RequireAuthorization();

// Client History
app.MapGet("/ordering/my-bookings", async (IQuerySession session, System.Security.Claims.ClaimsPrincipal user) =>
{
    var userId = user.GetUserId();
    if (!userId.HasValue) return Results.Unauthorized();

    var bookings = await session.Query<Booking>()
        .Where(b => b.UserId == userId.Value)
        .ToListAsync();

    return Results.Ok(bookings);
}).RequireAuthorization();

// Actions
app.MapPost("/ordering/bookings/{id}/accept", async (IMessageBus bus, Guid id) =>
{
    // In real app, verify Vendor owns this booking
    await bus.InvokeAsync(
        new Toyana.Contracts.ApproveBooking(id, Guid.Empty)); // Guid.Empty for VendorId if not checking yet
    return Results.Accepted();
}).RequireAuthorization();

app.MapPost("/ordering/bookings/{id}/reject", async (IMessageBus bus, Guid id) =>
{
    await bus.InvokeAsync(new Toyana.Contracts.RejectBooking(id, Guid.Empty, "Rejected by vendor"));
    return Results.Accepted();
}).RequireAuthorization();

app.Run();