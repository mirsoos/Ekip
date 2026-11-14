using Ekip.Application.Interfaces;
using StackExchange.Redis;

namespace Ekip.Infrastructure.Caching;

internal sealed class RedisIdempotencyService(IConnectionMultiplexer connectionMultiplexer)
    : IIdempotencyService
{
    private IDatabase Database => connectionMultiplexer.GetDatabase();

    public async Task<bool> TryAcquireLockAsync(
        string lockKey,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        cancellationToken.ThrowIfCancellationRequested();

        return await Database.StringSetAsync(
            lockKey,
            "locked",
            ttl,
            When.NotExists);
    }

    public async Task ReleaseLockAsync(string lockKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        cancellationToken.ThrowIfCancellationRequested();

        await Database.KeyDeleteAsync(lockKey);
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