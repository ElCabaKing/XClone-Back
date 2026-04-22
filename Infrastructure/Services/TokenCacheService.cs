

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
        var key = $"{entity.Purpose}:{entity.UserId}";
        var ttl = entity.ExpiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            return;
        }

        var value = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, value, ttl);
    }

    public async Task<T?> GetAndDeleteAsync(T entity)
    {
        var key = $"{entity.Purpose}:{entity.UserId}";
        var value = await _database.StringGetAsync(key);

        if (!value.HasValue)
            return default;

        await _database.KeyDeleteAsync(key); // Elimina después de leer

        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task<bool> ExistsAsync(string purpose, Guid userId)
    {
        var key = $"{purpose}:{userId}";
        return await _database.KeyExistsAsync(key);
    }



}