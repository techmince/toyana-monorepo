using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Toyana.Contracts;
using Toyana.Contracts.Security;
using Toyana.Shared;
using Toyana.Shared.Extensions;
using Toyana.VendorCenter.Data;
using Toyana.VendorCenter.Features.Vendors; // Handlers
using Wolverine;
using Wolverine.Http;
using Wolverine.RabbitMQ; // Observability

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.AddToyanaOpenApi();

// Observability
builder.AddToyanaObservability("vendor-center-api");
builder.AddToyanaJsonOptions();

// EF Core
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<VendorDbContext>(opts => opts.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

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

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseToyanaObservability();

await app.ApplyMigrationAsync<VendorDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseToyanaOpenApi();

app.UseAuthentication();
app.UseAuthorization();

// Wolverine HTTP Endpoints
app.MapWolverineEndpoints(opts =>
{
});

app.Run();
