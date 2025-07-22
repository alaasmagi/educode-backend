using StackExchange.Redis;

namespace App.DAL.EF;

public class CacheRepository(IConnectionMultiplexer redis)
{
    private readonly IDatabase redis = redis.GetDatabase();

    public async Task<bool> SetOtpAsync(Guid userId, string otp)
    {
        var key = $"otp:{userId}";
        var ttl = TimeSpan.FromMinutes(5); // TODO: get TTL from ENV!
        return await redis.StringSetAsync(key, otp, ttl);
    }

    public async Task<string?> GetOtpAsync(Guid userId)
    {
        var key = $"otp:{userId}";
        var otp = await redis.StringGetAsync(key);
        return otp.HasValue ? otp.ToString() : null;
    }
}