using Ekip.Application.Interfaces;
using Ekip.Domain.Entities;


namespace Ekip.Infrastructure.Repositories
{
    public class PostgreMessageRepository : IMessageReadRepository
    {
        public Task<List<Message>> GetMessagesAsync(long chatRoomId, int take = 50, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
