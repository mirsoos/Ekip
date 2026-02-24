namespace Ekip.Infrastructure.Services.Redis
{
    public interface IRedisService
    {
        Task SetUserOnlineAsync(Guid userRef , string connectionId);
        Task SetUserOfflineAsync(Guid userRef , string connectionId);
        Task<bool> IsUserOnlineAsync(Guid userRef);
        Task<List<Guid>> GetOnlineUsersAsync(IEnumerable<Guid> userRefs);
    }
}
