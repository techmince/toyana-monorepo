using System.Text;
using JasperFx;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Toyana.Ordering.Features.Bookings;
using Toyana.Shared.Extensions;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;
using Wolverine.RabbitMQ; // Observability

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.AddToyanaOpenApi();

// Observability
builder.AddToyanaObservability("ordering-api");
builder.AddToyanaJsonOptions();

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

builder.Services.AddWolverineHttp();


var app = builder.Build();

app.UseToyanaObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseToyanaOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Toyana.Ordering Service");

// Wolverine HTTP Endpoints
app.MapWolverineEndpoints(opts =>
{
});

app.Run();