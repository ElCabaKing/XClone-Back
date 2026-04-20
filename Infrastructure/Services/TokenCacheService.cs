

using Application.Interfaces;
using Domain.Exceptions;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services;


public class TokenCacheService<T>(IConnectionMultiplexer connectionMultiplexer) : ITokenCacheServiceGeneric<T> where T : ITokenCacheEntity
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    private readonly IServer _server = connectionMultiplexer.GetServer(
        connectionMultiplexer.GetEndPoints().FirstOrDefault()
        ?? throw new BadConfigurationException("Redis endpoint is not configured."));

    public async Task SaveAsync(T entity)
    {
        var key = $"{entity.UserId}:{entity.Purpose}";
        var ttl = entity.ExpiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            return;
        }

        var value = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, value, ttl);
    }

    public async Task<T?> VerifyAndConsumeAsync(Guid userId)
    {
        var pattern = $"{userId}:*";

        foreach (var redisKey in _server.Keys(_database.Database, pattern))
        {
            var cachedValue = await _database.StringGetDeleteAsync(redisKey);
            if (!cachedValue.HasValue)
            {
                continue;
            }

            return JsonSerializer.Deserialize<T>(cachedValue.ToString());
        }

        return default;
    }
}