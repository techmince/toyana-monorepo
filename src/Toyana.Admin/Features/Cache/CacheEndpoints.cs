using StackExchange.Redis;

namespace Toyana.Admin.Features.Cache;

public static class CacheEndpoints
{
    public static void MapCacheEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/cache/flush", async (IConnectionMultiplexer redis) => 
        {
            var server = redis.GetServer(redis.GetEndPoints().First());
            await server.FlushDatabaseAsync();
            return Results.Ok("Cache Flushed");
        });
    }
}
