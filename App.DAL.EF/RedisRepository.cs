using System.Text.Json;
using App.Domain;
using StackExchange.Redis;

namespace App.DAL.EF;

public class RedisRepository(IConnectionMultiplexer connection)
{
    private readonly IDatabase _database = connection.GetDatabase();
    
    public async Task<bool> SetRefreshTokenAsync(string token, Guid userId, string creatorIp, TimeSpan expires)
    {
        var tokenData = new RefreshTokenEntity
        {
            UserId = userId,
            Token = token,
            CreatedByIp = creatorIp,
        };
        
        var json = JsonSerializer.Serialize(tokenData);

        return await _database.StringSetAsync(token, json, expires);
    }

    public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token)
    {
        var value = await _database.StringGetAsync(token);
        if (value.IsNullOrEmpty) return null;

        return JsonSerializer.Deserialize<RefreshTokenEntity>(value!);
    }

    public async Task DeleteRefreshTokenAsync(string token)
    {
        await _database.KeyDeleteAsync(token);
    }
}