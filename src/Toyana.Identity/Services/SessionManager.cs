using StackExchange.Redis;

namespace Toyana.Identity.Services;

public interface ISessionManager
{
    Task RegisterSessionAsync(Guid userId, string sessionId);
    Task<bool> ValidateSessionAsync(Guid userId, string sessionId);
    Task RevokeSessionAsync(Guid userId, string sessionId);
}

public class SessionManager : ISessionManager
{
    private readonly IConnectionMultiplexer _redis;
    private const int MaxSessions = 2;
    private const string KeyPrefix = "session:user:";

    public SessionManager(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task RegisterSessionAsync(Guid userId, string sessionId)
    {
        var db = _redis.GetDatabase();
        var key = $"{KeyPrefix}{userId}";

        // Add new session to the list (RightPush)
        await db.ListRightPushAsync(key, sessionId);

        // Check if we exceeded the limit
        var count = await db.ListLengthAsync(key);
        if (count > MaxSessions)
        {
            // Remove the oldest session (LeftPop)
            var oldestSession = await db.ListLeftPopAsync(key);
            
            // Optionally: If we stored session data (like Refresh Tokens) in a separate key, 
            // we should delete that too. 
            // For now, we assume the presence in this list IS the validation.
            // If we have a separate key for token metadata:
            if (!oldestSession.IsNull)
            {
               await db.KeyDeleteAsync($"session:data:{oldestSession}");
            }
        }
        
        // Set expiry for the user session list to keep clean if inactive (e.g. 30 days)
        await db.KeyExpireAsync(key, TimeSpan.FromDays(30));
    }

    public async Task<bool> ValidateSessionAsync(Guid userId, string sessionId)
    {
        var db = _redis.GetDatabase();
        var key = $"{KeyPrefix}{userId}";
        
        // Scan the list or just check existence? Redis List doesn't have efficient "Contains".
        // For small N (2), getting the whole range is fine.
        var sessions = await db.ListRangeAsync(key);
        foreach (var s in sessions)
        {
            if (s == sessionId) return true;
        }
        return false;
    }

    public async Task RevokeSessionAsync(Guid userId, string sessionId)
    {
         var db = _redis.GetDatabase();
         var key = $"{KeyPrefix}{userId}";
         await db.ListRemoveAsync(key, sessionId);
    }
}
