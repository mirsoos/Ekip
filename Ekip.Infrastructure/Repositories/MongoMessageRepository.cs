using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;

namespace Ekip.Infrastructure.Repositories
{
    public class MongoMessageRepository : IMessageWriteRepository
    {
        public Task<Message> AddAsync(Message message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
