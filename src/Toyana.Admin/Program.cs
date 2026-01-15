using Marten; // Marten extensions
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using EF = Microsoft.EntityFrameworkCore; // Alias EF to avoid ambiguity
using System.Text;
using Toyana.Identity.Data; // Identity DbContext
using Toyana.VendorCenter.Data; // Vendor DbContext
using Toyana.Ordering.Features.Bookings; // Booking Logic
using Toyana.Shared;
using Wolverine;
using Wolverine.RabbitMQ;
// using Microsoft.OpenApi.Models; // Removed to fix build

using Toyana.Shared.Extensions; // Observability
using Toyana.Admin.Features.Dashboard;
using Toyana.Admin.Features.Users;
using Toyana.Admin.Features.Vendors;
using Toyana.Admin.Features.Bookings;
using Toyana.Admin.Features.Payments;
using Toyana.Admin.Features.Cache;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Basic config only

// Observability
builder.AddToyanaObservability("admin-api");

// --- DATABASES (READ ACCESS) ---

// 1. Identity DB (EF Core)
var identityConn = builder.Configuration.GetConnectionString("IdentityConnection") 
                   ?? "Host=localhost;Port=5432;Database=toyana_identity;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(identityConn));

// 2. Vendor DB (EF Core)
var vendorConn = builder.Configuration.GetConnectionString("VendorConnection") 
                 ?? "Host=localhost;Port=5432;Database=toyana_vendor;Username=postgres;Password=postgres";
builder.Services.AddDbContext<VendorDbContext>(options =>
    options.UseNpgsql(vendorConn));

// 3. Ordering DB (Marten)
var orderingConn = builder.Configuration.GetConnectionString("OrderingConnection") 
                   ?? "Host=localhost;Port=5432;Database=toyana_ordering;Username=postgres;Password=postgres";
builder.Services.AddMarten(opts =>
{
    opts.Connection(orderingConn);
    opts.DatabaseSchemaName = "ordering"; 
    // We only need to read Bookings here. We assume schemas match.
});

// 4. Cache (Valkey)
var valkeyConn = builder.Configuration.GetConnectionString("Valkey") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(valkeyConn));

// --- AUTHENTICATION ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyForToyanaProjectAndItMustBeLongEnough";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Toyana",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Toyana",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// --- WOLVERINE (COMMANDS) ---
builder.Host.UseWolverine(opts =>
{
    var rabbitConn = builder.Configuration.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@localhost:5672";
    opts.UseRabbitMq(new Uri(rabbitConn));
    // Admin sends commands to other services. 
    // We rely on message routing defined by contracts if possible, or explicit routing.
    // Explicit routing for safety:
    opts.PublishMessage<Toyana.Contracts.RejectBooking>().ToRabbitQueue("toyana.ordering");
    // Add other commands as needed
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

// --- ADMIN ENDPOINTS ---

// Ensure Admin Role
var adminGroup = app.MapGroup("/admin").RequireAuthorization(policy => 
    policy.RequireRole("Admin"));

adminGroup.MapDashboardEndpoints();
adminGroup.MapUsersEndpoints();
adminGroup.MapVendorsEndpoints();
adminGroup.MapBookingsEndpoints();
adminGroup.MapPaymentsEndpoints();
adminGroup.MapCacheEndpoints();

app.Run();
