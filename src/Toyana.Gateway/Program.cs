using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Toyana.Shared.Extensions;

// Observability

var builder = WebApplication.CreateBuilder(args);

// Observability
builder.AddToyanaObservability("gateway-api");
builder.AddToyanaJsonOptions();

// Add services to the container.
var jwtKey      = builder.Configuration["Jwt:Key"]      ?? "ThisIsASecretKeyForToyanaProjectAndItMustBeLongEnough";
var jwtIssuer   = builder.Configuration["Jwt:Issuer"]   ?? "Toyana";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Toyana";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
                     {
                         options.TokenValidationParameters = new TokenValidationParameters
                                                             {
                                                                 ValidateIssuer           = true,
                                                                 ValidateAudience         = true,
                                                                 ValidateLifetime         = true,
                                                                 ValidateIssuerSigningKey = true,
                                                                 ValidIssuer              = jwtIssuer,
                                                                 ValidAudience            = jwtAudience,
                                                                 IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                                                             };
                     });

builder.Services.AddAuthorization(options =>
                                  {
                                      options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
                                      options.AddPolicy("RequireVendor", policy => policy.RequireRole("Vendor"));
                                  });

builder.Services.AddReverseProxy()
       .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseToyanaObservability();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapScalarApiReference(options =>
                          {
                              options.WithTitle("Toyana API Gateway")
                                     .WithTheme(ScalarTheme.DeepSpace)
                                     .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

                              options.AddDocument("auth",     "auth",     "/auth/openapi/v1.json");
                              options.AddDocument("vendors",  "vendors",  "/vendors/openapi/v1.json");
                              options.AddDocument("bookings", "bookings", "/bookings/openapi/v1.json");
                              options.AddDocument("catalog",  "catalog",  "/catalog/openapi/v1.json");
                              options.AddDocument("chat",     "chat",     "/chat/openapi/v1.json");
                              options.AddDocument("admin",    "admin",    "/admin/openapi/v1.json");
                              options.AddDocument("alerts",   "alerts",   "/alerts/openapi/v1.json");
                              options.AddDocument("payments", "payments", "/payments/openapi/v1.json");
                          });

app.MapGet("/", () => "Toyana.Gateway Running");

app.Run();