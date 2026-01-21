using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using Wolverine.Http;

namespace Toyana.Admin.Features.Cache;

public static class CacheEndpoints
{
    [WolverinePost("/admin/cache/flush")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> FlushCache(IConnectionMultiplexer redis)
    {
        var server = redis.GetServer(redis.GetEndPoints().First());
        await server.FlushDatabaseAsync();
        return Results.Ok("Cache Flushed");
    }
}