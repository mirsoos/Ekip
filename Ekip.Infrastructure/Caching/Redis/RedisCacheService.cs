using System.Text.Json;
using Ekip.Application.Interfaces;
using StackExchange.Redis;

namespace Ekip.Infrastructure.Caching.Redis;

internal sealed class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private IDatabase Database => connectionMultiplexer.GetDatabase();

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

    public async Task<long> IncrementAsync(
        string key,
        TimeSpan? expirationIfNew = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        cancellationToken.ThrowIfCancellationRequested();

        var count = await Database.StringIncrementAsync(key);
        if (count == 1 && expirationIfNew.HasValue)
            await Database.KeyExpireAsync(key, expirationIfNew.Value);

        return count;
    }
}