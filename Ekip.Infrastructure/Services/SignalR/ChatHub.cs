using Ekip.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Ekip.Infrastructure.Services.SignalR
{

    [Authorize] 
    public class ChatHub : Hub
    {
    private readonly IChatRoomReadRepository _chatRoomRead;
        public ChatHub(IChatRoomReadRepository chatRoomRead)
        {
            _chatRoomRead = chatRoomRead;
        }

        public async Task JoinRoom(Guid chatRoomId)
        {
            var userId = Context.UserIdentifier;

            var isParticipant = await _chatRoomRead.IsUserParticipant(chatRoomId, Guid.Parse(userId) , CancellationToken.None);

            if (!isParticipant)
            {
                throw new HubException("Access Denied.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        }
        public async Task LeaveRoom(Guid chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        }
    }
}
