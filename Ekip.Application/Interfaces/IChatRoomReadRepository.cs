using Ekip.Application.DTOs.ChatRoom;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IChatRoomReadRepository
    {
        Task<ChatRoomReadModel> AddChatRoomAsync(ChatRoomReadModel chatRoomReadModel,CancellationToken cancellationToken);
        Task<ChatRoomReadModel?> GetByIdAsync(Guid chatRoomRef, CancellationToken cancellationToken);
        Task<ChatRoomListDto?> GetListByIdAsync(Guid chatRoomRef, CancellationToken cancellationToken);
        Task UpdateLastMessageAsync(Guid chatRoomRef, string LastMessagePreview, DateTime LastMessageDate,CancellationToken cancellationToken);
        Task<bool> IsUserParticipant(Guid chatRoomRef , Guid userRef , CancellationToken cancellationToken);
        Task<List<Guid>> GetUserRoomIds(Guid userRef, CancellationToken cancellationToken);
    }
}
