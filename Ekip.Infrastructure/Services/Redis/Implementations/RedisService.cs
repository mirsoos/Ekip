using Ekip.Infrastructure.Persistence.Redis;
using Ekip.Infrastructure.Services.Redis.Interfaces;
using StackExchange.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SetUserOnlineAsync(Guid userId, string connectionId)
    {
        var key = KeySchema.PresenceKey(userId);
        var tran = _db.CreateTransaction();
        _ = tran.SetAddAsync(key, connectionId);
        _ = tran.KeyExpireAsync(key, TimeSpan.FromHours(24));
        await tran.ExecuteAsync();
    }

    public async Task<bool> SetUserOfflineAsync(Guid userId, string connectionId)
    {
        var key = KeySchema.PresenceKey(userId);
        var luaScript = @"
            redis.call('SREM', KEYS[1], ARGV[1])
            local count = redis.call('SCARD', KEYS[1])
            if count == 0 then
                redis.call('EXPIRE', KEYS[1], 60)
            end
            return count";

        var remaining = (long)await _db.ScriptEvaluateAsync(luaScript,
            new RedisKey[] { key }, new RedisValue[] { connectionId });

        return remaining == 0;
    }

    public async Task AddUserRoomToCacheAsync(Guid userId, Guid roomId)
    {
        var key = KeySchema.UserRoomsKey(userId);
        if (await _db.KeyExistsAsync(key))
        {
            await _db.SetAddAsync(key, roomId.ToString());
        }
    }

    public async Task<List<Guid>> GetOnlineUsersAsync(IEnumerable<Guid> userRefs)
    {
        var userList = userRefs.ToList();
        if (!userList.Any()) return new List<Guid>();
        var batch = _db.CreateBatch();
        var tasks = userList.Select(u => batch.KeyExistsAsync(KeySchema.PresenceKey(u))).ToList();
        batch.Execute();
        var results = await Task.WhenAll(tasks);
        return userList.Where((_, index) => results[index]).ToList();
    }

    public async Task<bool> IsUserOnlineAsync(Guid userId) => await _db.KeyExistsAsync(KeySchema.PresenceKey(userId));

    public async Task CacheUserRoomsAsync(Guid userId, IEnumerable<Guid> roomIds)
    {
        var key = KeySchema.UserRoomsKey(userId);
        var roomValues = roomIds.Select(r => (RedisValue)r.ToString()).ToArray();
        var tran = _db.CreateTransaction();
        _ = tran.KeyDeleteAsync(key);
        if (roomValues.Length > 0)
        {
            _ = tran.SetAddAsync(key, roomValues);
            _ = tran.KeyExpireAsync(key, TimeSpan.FromDays(7));
        }
        await tran.ExecuteAsync();
    }

    public async Task<IEnumerable<Guid>> GetUserRoomsFromCacheAsync(Guid userId)
    {
        var key = KeySchema.UserRoomsKey(userId);
        var rooms = await _db.SetMembersAsync(key);
        return rooms.Select(r => Guid.Parse(r!));
    }

    public async Task<bool> RemoveUserRoomFromCacheAsync(Guid userRef, Guid chatroomRef)
    {
        var key = KeySchema.UserRoomsKey(userRef);

        var luaScript = @"
        local removed = redis.call('SREM', KEYS[1], ARGV[1])
        local count = redis.call('SCARD', KEYS[1])
        
        if count == 0 then
            redis.call('DEL', KEYS[1])
        end        
        return removed
        ";

        var result = await _db.ScriptEvaluateAsync(
            luaScript,
            new RedisKey[] { key },
            new RedisValue[] { chatroomRef.ToString() }
        );

        return (bool)result;
    }
}