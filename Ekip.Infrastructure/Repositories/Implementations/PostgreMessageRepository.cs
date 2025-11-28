using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;


namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class PostgreMessageRepository : IMessageReadRepository
    {
        public Task<List<Message>> GetMessagesAsync(long chatRoomId, int take = 50, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
