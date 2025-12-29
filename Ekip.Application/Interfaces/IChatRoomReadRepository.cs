using Ekip.Application.DTOs.ChatRoom;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IChatRoomReadRepository
    {
        Task<ChatRoomReadModel> AddChatRoomAsync(ChatRoomReadModel chatRoomReadModel,CancellationToken cancellationToken);
        Task<ChatRoomReadModel?> GetByIdAsync(long chatRoomRef, CancellationToken cancellationToken);
        Task<ChatRoomListDto?> GetListByIdAsync(long chatRoomRef, CancellationToken cancellationToken);
        Task UpdateLastMessageAsync(long chatRoomRef, string LastMessagePreview, DateTime LastMessageDate,CancellationToken cancellationToken);
    }
}
