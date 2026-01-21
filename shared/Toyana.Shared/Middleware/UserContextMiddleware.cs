using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Toyana.Shared.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId        = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userType      = context.User?.FindFirst(ClaimTypes.Role)?.Value;
        var correlationId = context.TraceIdentifier; // Or get from header if CorrelationIdMiddleware runs before

        // Push properies to Serilog LogContext
        using (LogContext.PushProperty("UserId", userId ?? "Anonymous"))
        using (LogContext.PushProperty("UserType", userType ?? "Guest"))
        {
            await _next(context);
        }
    }
}