using Ekip.Application.DTOs.ChatRoom;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ChatRoomReadRepository : IChatRoomReadRepository
    {
        public Task<ChatRoomReadModel> AddChatRoomAsync(ChatRoomReadModel chatRoomReadModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ChatRoom?> GetByIdAsync(long chatRoomRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ChatRoomListDto?> GetListByIdAsync(long chatRoomRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ChatRoomReadModel?> IChatRoomReadRepository.GetByIdAsync(long chatRoomRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
