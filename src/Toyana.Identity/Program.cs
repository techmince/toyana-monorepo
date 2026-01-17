using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Toyana.Identity.Data;
using Toyana.Identity.Models;
using Toyana.Identity.Services;
using Toyana.Shared.Extensions; // Observability
using Wolverine;
using Wolverine.Http;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.AddToyanaOpenApi();

// Observability
builder.AddToyanaObservability("identity-api");
builder.AddToyanaJsonOptions();

// EF Core
var dbConn = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dbConn)
        .UseSnakeCaseNamingConvention(CultureInfo.InvariantCulture)
    );

// Redis/Valkey
var redisConn = builder.Configuration.GetConnectionString("Valkey") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConn));
builder.Services.AddSingleton<ISessionManager, SessionManager>();

// Auth Service
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<AuthService>();

// JWT Authentication
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

// Wolverine
builder.Host.UseWolverine(opts =>
{
    var rabbitConn = builder.Configuration.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@localhost:5672";
    opts.UseRabbitMq(new Uri(rabbitConn)).AutoProvision();
    opts.PublishAllMessages().ToRabbitQueue("toyana.vendor"); 
});

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseToyanaObservability();

// Apply Migrations always (not just dev) for this setup request, or keep consistent?
// User said "ensure all migrations are setup on startup".
await app.ApplyMigrationAsync<ApplicationDbContext>();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
    // Seed Admin (Idempotent check)
    if (!await db.AdminUsers.AnyAsync())
    {
        db.AdminUsers.Add(new AdminUser 
        { 
            Id = Guid.NewGuid(), 
            Username = "admin", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") 
        });
        await db.SaveChangesAsync();
    }
}

app.UseToyanaOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Wolverine HTTP Endpoints
app.MapWolverineEndpoints(opts =>
{
    // opts.UseFluentValidationProblemDetailMiddleware(); // If we add FluentValidation later
});

app.Run();
