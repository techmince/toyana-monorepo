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
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Observability
builder.AddToyanaObservability("identity-api");

// EF Core
var dbConn = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dbConn));

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

var app = builder.Build();

app.UseToyanaObservability();

// Apply Migrations always (not just dev) for this setup request, or keep consistent?
// User said "ensure all migrations are setup on startup".
await app.ApplyMigrationAsync<ApplicationDbContext>();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// --- CLIENT ENDPOINTS ---
app.MapPost("/auth/client/register", async (AuthService auth, RegisterRequest request) => 
{
    try { return Results.Ok(await auth.RegisterClientAsync(request)); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});

app.MapPost("/auth/client/login", async (AuthService auth, LoginRequest request) => 
{
    try { return Results.Ok(await auth.LoginClientAsync(request)); }
    catch (Exception ex) { return Results.Unauthorized(); }
});

// --- VENDOR ENDPOINTS ---
app.MapPost("/auth/vendor/register", async (AuthService auth, VendorRegisterRequest request) => 
{
    try { return Results.Ok(await auth.RegisterVendorOwnerAsync(request)); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});

app.MapPost("/auth/vendor/login", async (AuthService auth, LoginRequest request) => 
{
    try { return Results.Ok(await auth.LoginVendorAsync(request)); }
    catch (Exception ex) { return Results.Unauthorized(); }
});

// --- ADMIN ENDPOINTS ---
app.MapPost("/auth/admin/login", async (AuthService auth, LoginRequest request) => 
{
    try { return Results.Ok(await auth.LoginAdminAsync(request)); }
    catch (Exception ex) { return Results.Unauthorized(); }
});

// Sub-user creation (Owner only)
app.MapPost("/auth/vendor/users", async (HttpContext context, AuthService auth, CreateSubUserRequest request) => 
{
    var userIdString = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        return Results.Unauthorized();

    try { return Results.Ok(await auth.CreateSubUserAsync(userId, request)); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
}).RequireAuthorization(); // Requires Valid Token

app.Run();
