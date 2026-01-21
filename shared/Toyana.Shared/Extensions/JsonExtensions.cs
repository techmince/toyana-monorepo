using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Toyana.Shared.Extensions;

public static class JsonExtensions
{
    public static WebApplicationBuilder AddToyanaJsonOptions(this WebApplicationBuilder builder)
    {
        // Configure Minimal APIs
        builder.Services.Configure<JsonOptions>(options =>
                                                {
                                                    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                                                    options.SerializerOptions.DictionaryKeyPolicy  = JsonNamingPolicy.SnakeCaseLower;
                                                });

        // Configure Controllers (if any are used)
        builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
                                                                         {
                                                                             options.JsonSerializerOptions.PropertyNamingPolicy =
                                                                                 JsonNamingPolicy.SnakeCaseLower;
                                                                             options.JsonSerializerOptions.DictionaryKeyPolicy =
                                                                                 JsonNamingPolicy.SnakeCaseLower;
                                                                         });

        return builder;
    }

    public static IHostApplicationBuilder AddToyanaJsonOptions(this IHostApplicationBuilder builder)
    {
        // Configure Minimal APIs
        builder.Services.Configure<JsonOptions>(options =>
                                                {
                                                    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                                                    options.SerializerOptions.DictionaryKeyPolicy  = JsonNamingPolicy.SnakeCaseLower;
                                                });

        // Configure Controllers (if any are used)
        builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
                                                                         {
                                                                             options.JsonSerializerOptions.PropertyNamingPolicy =
                                                                                 JsonNamingPolicy.SnakeCaseLower;
                                                                             options.JsonSerializerOptions.DictionaryKeyPolicy =
                                                                                 JsonNamingPolicy.SnakeCaseLower;
                                                                         });

        return builder;
    }
}