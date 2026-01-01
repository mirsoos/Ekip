using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Infrastructure.Persistence;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class MessageWriteRepository : IMessageWriteRepository
    {
        private readonly MongoDbContext _mongoDb;
        public MessageWriteRepository(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }
        public async Task<Message> AddAsync(Message message, CancellationToken cancellationToken)
        {
            await _mongoDb.Messages.InsertOneAsync(message, null, cancellationToken);
            return message;
        }
    }
}
