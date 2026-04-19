using Ekip.Application.Interfaces;
using Ekip.Infrastructure.Services.Redis.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Ekip.Infrastructure.Services.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRoomReadRepository _chatRoomRead;
        private readonly IRedisService _redisService;

        public ChatHub(IChatRoomReadRepository chatRoomRead, IRedisService redisService)
        {
            _chatRoomRead = chatRoomRead;
            _redisService = redisService;
        }

        private Guid UserId => Guid.Parse(Context.UserIdentifier!);

        public override async Task OnConnectedAsync()
        {
            var userId = UserId;
            var connectionId = Context.ConnectionId;

            await _redisService.SetUserOnlineAsync(userId, connectionId);

            var userRoomIds = await _redisService.GetUserRoomsFromCacheAsync(userId);

            if (!userRoomIds.Any())
            {
                userRoomIds = await _chatRoomRead.GetUserRoomIds(userId, Context.ConnectionAborted);
                await _redisService.CacheUserRoomsAsync(userId, userRoomIds);
            }

            var joinTasks = userRoomIds.Select(roomId =>
                Groups.AddToGroupAsync(connectionId, roomId.ToString()));

            await Task.WhenAll(joinTasks);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var isLastDevice = await _redisService.SetUserOfflineAsync(UserId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(Guid chatRoomId)
        {
            var userId = UserId;

            var isParticipant = await _chatRoomRead.IsUserParticipant(chatRoomId, userId, Context.ConnectionAborted);

            if (!isParticipant)
            {
                throw new HubException("Access Denied.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());

            await _redisService.AddUserRoomToCacheAsync(userId, chatRoomId);
        }

        public async Task LeaveRoom(Guid chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
            await _redisService.RemoveUserRoomFromCacheAsync(UserId, chatRoomId);
        }
    }
}