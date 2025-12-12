using Ekip.Application.DTOs.ChatRoom;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IChatRoomReadRepository
    {
        Task<ChatRoomReadModel?> GetByIdAsync(long chatRoomRef, CancellationToken cancellationToken);
        Task<ChatRoomReadModel> AddChatRoomAsync(ChatRoomReadModel chatRoomReadModel,CancellationToken cancellationToken);
        Task<ChatRoomListDto?> GetListByIdAsync(long chatRoomRef, CancellationToken cancellationToken);
    }
}
