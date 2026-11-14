namespace Ekip.Application.Interfaces;

public interface IIdempotencyService
{
    Task<bool> TryAcquireLockAsync(
        string lockKey,
        TimeSpan ttl,
        CancellationToken cancellationToken = default);

    Task ReleaseLockAsync(
        string lockKey,
        CancellationToken cancellationToken = default);

    Task<string?> GetCachedResultAsync(
        string cacheKey,
        CancellationToken cancellationToken = default);

    Task SetCachedResultAsync(
        string cacheKey,
        string serializedResponse,
        TimeSpan ttl,
        CancellationToken cancellationToken = default);
}