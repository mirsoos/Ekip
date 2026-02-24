using StackExchange.Redis;

namespace Ekip.Infrastructure.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redis;
        private const string keyPrefix = "presence:";
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }
        public async Task<List<Guid>> GetOnlineUsersAsync(IEnumerable<Guid> userRefs)
        {
            var onlineUsers = new List<Guid>();
            foreach (var userRef in userRefs) 
            {
                if(await IsUserOnlineAsync(userRef)) onlineUsers.Add(userRef);
            }
            return onlineUsers;
        }

        public async Task<bool> IsUserOnlineAsync(Guid userRef)
        {
            string key = $"{keyPrefix}{userRef}";
            return await _redis.SetLengthAsync(key) > 0;
        }

        public async Task SetUserOfflineAsync(Guid userRef, string connectionId)
        {
            string key = $"{keyPrefix}{userRef}";
            await _redis.SetRemoveAsync(key, connectionId);
        }

        public async Task SetUserOnlineAsync(Guid userRef, string connectionId)
        {
            string key = $"{keyPrefix}{userRef}";

            await _redis.SetAddAsync(key, connectionId);
            await _redis.KeyExpireAsync(key, TimeSpan.FromHours(24));
        }
    }
}
