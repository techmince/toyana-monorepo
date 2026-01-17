using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql; // For AddNpgsql
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Toyana.Shared.Middleware; // Potentially needed if not in Serilog namespace

namespace Toyana.Shared.Extensions;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddToyanaObservability(this WebApplicationBuilder builder, string serviceName)
    {
        ConfigureLogger(serviceName);
        builder.Host.UseSerilog();
        ConfigureOpenTelemetry(builder.Services, serviceName);
        return builder;
    }

    public static Microsoft.Extensions.Hosting.IHostApplicationBuilder AddToyanaObservability(this Microsoft.Extensions.Hosting.IHostApplicationBuilder builder, string serviceName)
    {
        ConfigureLogger(serviceName);
        builder.Services.AddSerilog();
        ConfigureOpenTelemetry(builder.Services, serviceName);
        return builder;
    }

    private static void ConfigureLogger(string serviceName)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithSpan() 
            .WriteTo.Console()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = "http://otel-collector:4318/v1/logs"; 
                options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName
                };
            })
            .CreateLogger();
    }

    private static void ConfigureOpenTelemetry(IServiceCollection services, string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql() // Postgres Tracing
                    .AddOtlpExporter(opts => 
                    {
                        opts.Endpoint = new Uri("http://otel-collector:4317");
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(opts => 
                    {
                        opts.Endpoint = new Uri("http://otel-collector:4317");
                    });
            });

        // Add Global Exception Handler
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    public static WebApplication UseToyanaObservability(this WebApplication app)
    {
        app.UseExceptionHandler(); // Uses GlobalExceptionHandler
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<UserContextMiddleware>();
        // Add Serilog Request Logging manually if preferred or rely on OpenTelemetry traces
        // Serilog Request Logging is good for summarized logs.
        app.UseSerilogRequestLogging(cfg =>
        {
            cfg.IncludeQueryInRequestPath = true;
        });

        return app;
    }
}
