using Ekip.Domain.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IChatRoomReadRepository
    {
        Task<ChatRoom?> GetByIdAsync(long chatRoomId, CancellationToken cancellationToken);
    }
}
