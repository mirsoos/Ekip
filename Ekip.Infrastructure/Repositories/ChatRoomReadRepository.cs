using Ekip.Application.Interfaces;
using Ekip.Domain.Entities;

namespace Ekip.Infrastructure.Repositories
{
    public class ChatRoomReadRepository : IChatRoomReadRepository
    {
        public Task<Chatroom?> GetByIdAsync(long chatRoomId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
