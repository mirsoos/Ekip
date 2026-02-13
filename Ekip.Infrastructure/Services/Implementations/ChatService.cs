using Ekip.Application.Interfaces;
using Ekip.Infrastructure.Services.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Ekip.Infrastructure.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendMessageToGroup(Guid chatRoomId, object messageData)
        {
            await _hubContext.Clients
                .Group(chatRoomId.ToString())
                .SendAsync("ReceiveMessage", messageData);
        }
    }
}
