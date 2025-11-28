using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ChatRoomReadRepository : IChatRoomReadRepository
    {
        public Task<ChatRoom?> GetByIdAsync(long chatRoomId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
