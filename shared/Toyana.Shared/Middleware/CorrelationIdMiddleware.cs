using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using System.Diagnostics;

namespace Toyana.Shared.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        // Ensure the header is present in the response
        if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
        {
            context.Response.Headers[CorrelationIdHeaderName] = correlationId;
        }

        // Push to Serilog Context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            // Push to Activity (Trace) if enabled
            var activity = Activity.Current;
            if (activity != null)
            {
                activity.SetTag("correlation_id", correlationId);
                // Also could set formatting for trace propagation if strictly needed, 
                // but OTLP handles traceparent/tracestate usually.
            }

            await _next(context);
        }
    }

    private string GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out StringValues correlationId))
        {
            return correlationId.ToString();
        }

        return Guid.NewGuid().ToString();
    }
}
