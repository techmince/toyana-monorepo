using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Toyana.Shared.Extensions;

public static class OpenApiExtensions
{
    public static WebApplicationBuilder AddToyanaOpenApi(this WebApplicationBuilder builder)
    {
        // Native .NET 10 OpenAPI generation
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplication UseToyanaOpenApi(this WebApplication app)
    {
        // Expose the generated OpenAPI document at /openapi/v1.json (default)
        app.MapOpenApi();

        return app;
    }
}