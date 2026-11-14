using Ekip.Application.Interfaces;
using StackExchange.Redis;

namespace Ekip.Infrastructure.Caching.Redis;

internal sealed class RedisIdempotencyService(IConnectionMultiplexer connectionMultiplexer)
    : IIdempotencyService
{
    private static readonly LuaScript ReleaseLockScript = LuaScript.Prepare(
        """
        if redis.call('GET', @key) == @owner then
            return redis.call('DEL', @key)
        end
        return 0
        """);

    private IDatabase Database => connectionMultiplexer.GetDatabase();

    public async Task<bool> TryAcquireLockAsync(
        string lockKey,
        string ownerToken,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerToken);
        cancellationToken.ThrowIfCancellationRequested();

        return await Database.StringSetAsync(
            lockKey,
            ownerToken,
            ttl,
            When.NotExists);
    }

    public async Task ReleaseLockAsync(
        string lockKey,
        string ownerToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerToken);
        cancellationToken.ThrowIfCancellationRequested();

        await Database.ScriptEvaluateAsync(
            ReleaseLockScript,
            new { key = (RedisKey)lockKey, owner = (RedisValue)ownerToken });
    }

    public async Task<string?> GetCachedResultAsync(
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);
        cancellationToken.ThrowIfCancellationRequested();

        var value = await Database.StringGetAsync(cacheKey);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task SetCachedResultAsync(
        string cacheKey,
        string serializedResponse,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);
        ArgumentNullException.ThrowIfNull(serializedResponse);
        cancellationToken.ThrowIfCancellationRequested();

        await Database.StringSetAsync(cacheKey, serializedResponse, ttl);
    }
}