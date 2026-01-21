using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Toyana.Shared.Middleware;

public class CorrelationIdMiddleware
{
    private const    string          TraceIdHeaderName       = "X-Trace-Id";
    private const    string          CorrelationIdHeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var traceId = GetTraceId(context);

        // Ensure the header is present in the response
        if (!context.Response.Headers.ContainsKey(TraceIdHeaderName)) context.Response.Headers[TraceIdHeaderName] = traceId;

        // Ensure the header is present in the request (for downstream propagation, e.g. YARP)
        if (!context.Request.Headers.ContainsKey(TraceIdHeaderName)) context.Request.Headers[TraceIdHeaderName] = traceId;

        // Push to Serilog Context
        // User requested "use it in structured logs". We use "TraceId" as the key.
        using (LogContext.PushProperty("TraceId", traceId))
        {
            // Also push CorrelationId for backward compatibility if needed, or just map both
            using (LogContext.PushProperty("CorrelationId", traceId))
            {
                // Push to Activity (Trace) if enabled
                var activity = Activity.Current;
                if (activity != null) activity.SetTag("trace_id", traceId);
                // Standard OpenTelemetry uses standard headers, but we tag custom ID too
                await _next(context);
            }
        }
    }

    private string GetTraceId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(TraceIdHeaderName, out var traceId)) return traceId.ToString();

        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId)) return correlationId.ToString();

        return Guid.NewGuid().ToString();
    }
}