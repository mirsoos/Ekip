namespace Ekip.Infrastructure.Services.Redis.Interfaces
{
    public interface IRedisService
    {
        Task SetUserOnlineAsync(Guid userId, string connectionId);
        Task<bool> SetUserOfflineAsync(Guid userId, string connectionId);
        Task<bool> IsUserOnlineAsync(Guid userId);
        Task<List<Guid>> GetOnlineUsersAsync(IEnumerable<Guid> userRefs);
        Task CacheUserRoomsAsync(Guid userId, IEnumerable<Guid> roomIds);
        Task<IEnumerable<Guid>> GetUserRoomsFromCacheAsync(Guid userId);
        Task AddUserRoomToCacheAsync(Guid userRef , Guid chatroomRef);
        Task<bool> RemoveUserRoomFromCacheAsync(Guid userRef , Guid chatroomRef);
    }
}