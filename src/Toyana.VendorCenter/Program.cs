using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wolverine;
using Wolverine.RabbitMQ;
using Toyana.Contracts;
using Toyana.Contracts.Security;
using Toyana.VendorCenter.Data;
using Toyana.VendorCenter.Features.Vendors; // Handlers
using Toyana.Shared;

using Toyana.Shared.Extensions; // Observability

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Observability
builder.AddToyanaObservability("vendor-center-api");

// EF Core
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<VendorDbContext>(opts => opts.UseNpgsql(connectionString));

// Auth (JWT)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyForToyanaProjectAndItMustBeLongEnough";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Toyana";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Toyana";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageServices", policy => 
        policy.RequireAssertion(context => 
            context.User.HasClaim("isOwner", "true") || 
            context.User.HasClaim("permission", VendorPermission.ManageServices)));

    options.AddPolicy("ManageAvailability", policy => 
        policy.RequireAssertion(context => 
            context.User.HasClaim("isOwner", "true") || 
            context.User.HasClaim("permission", VendorPermission.ManageAvailability)));
});


// Wolverine
builder.Host.UseWolverine(opts =>
{
    var rabbitConn = builder.Configuration.GetConnectionString("RabbitMq")!;
    opts.UseRabbitMq(new Uri(rabbitConn)).AutoProvision();
});

var app = builder.Build();

app.UseToyanaObservability();

await app.ApplyMigrationAsync<VendorDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Command Endpoints
// CreateVendor: Usually open or restricted to system. We'll leave open for now or Require "Vendor" role if called from specific flow.
app.MapPost("/vendors", async (IMessageBus bus, CreateVendor command) =>
{
    await bus.InvokeAsync(command);
    return Results.Accepted();
});

// Add Service: Requires "ManageServices"
app.MapPost("/vendors/services", async (IMessageBus bus, AddService command) =>
{
    await bus.InvokeAsync(command);
    return Results.Accepted();
}).RequireAuthorization("ManageServices");

// Set Availability: Requires "ManageAvailability"
app.MapPost("/vendors/availability", async (IMessageBus bus, SetAvailability command) =>
{
    await bus.InvokeAsync(command);
    return Results.Accepted();
}).RequireAuthorization("ManageAvailability");

// --- Read APIs ---

app.MapGet("/vendors/services", async (VendorDbContext db, System.Security.Claims.ClaimsPrincipal user) =>
{
    var vendorId = user.GetVendorId();
    if (!vendorId.HasValue) return Results.Unauthorized();

    var services = await db.Services.Where(s => s.VendorId == vendorId.Value).ToListAsync();
    return Results.Ok(services);
}).RequireAuthorization(); // Basic auth required, specific permissions handled by UI mostly, or could restrict to "ManageServices"

app.MapGet("/vendors/availability", async (VendorDbContext db, System.Security.Claims.ClaimsPrincipal user) =>
{
    var vendorId = user.GetVendorId();
    if (!vendorId.HasValue) return Results.Unauthorized();

    var slots = await db.AvailabilitySlots
        .Where(a => a.VendorId == vendorId.Value)
        .OrderBy(a => a.Date)
        .ToListAsync();
        
    return Results.Ok(slots);
}).RequireAuthorization();

app.Run();
