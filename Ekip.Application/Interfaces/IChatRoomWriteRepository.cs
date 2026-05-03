using Ekip.Domain.Entities.Chat.Entites;

namespace Ekip.Application.Interfaces
{
    public interface IChatRoomWriteRepository
    {
        Task<ChatRoom> AddChatRoomAsync(ChatRoom chatRoom,CancellationToken cancellationToken);
        Task<ChatRoom?> GetByIdAsync(Guid chatroomRef , CancellationToken cancellationToken);
    }
}
