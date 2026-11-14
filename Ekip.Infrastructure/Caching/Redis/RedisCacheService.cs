using System.Text.Json;
using Ekip.Application.Interfaces;
using StackExchange.Redis;

namespace Ekip.Infrastructure.Caching.Redis;

internal sealed class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private IDatabase Database => connectionMultiplexer.GetDatabase();

    private static readonly LuaScript IncrementWithExpiryScript = LuaScript.Prepare(
        """
        local current = redis.call('INCR', @key)
        if current == 1 or redis.call('TTL', @key) == -1 then
            redis.call('PEXPIRE', @key, @ttlMs)
        end
        return current
        """);

    public async Task<long> IncrementAsync(
        string key,
        TimeSpan? expirationIfNew = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        cancellationToken.ThrowIfCancellationRequested();

        if (!expirationIfNew.HasValue)
            return await Database.StringIncrementAsync(key);

        var ttlMs = Math.Max(1L, (long)expirationIfNew.Value.TotalMilliseconds);
        var result = await Database.ScriptEvaluateAsync(
            IncrementWithExpiryScript,
            new { key = (RedisKey)key, ttlMs });

        return (long)result;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        cancellationToken.ThrowIfCancellationRequested();

        var value = await Database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(((ReadOnlyMemory<byte>)value!).Span, JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        cancellationToken.ThrowIfCancellationRequested();

        var payload = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        await Database.StringSetAsync(key, payload, expiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        cancellationToken.ThrowIfCancellationRequested();

        await Database.KeyDeleteAsync(key);
    }
}