using Ekip.Domain.Entities.Chat.Entites;

namespace Ekip.Application.Interfaces
{
    public interface IChatRoomReadRepository
    {
        Task<ChatRoom?> GetByIdAsync(long chatRoomId, CancellationToken cancellationToken);
    }
}
