using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Outbox;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using System.Text.Json;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class OutboxEventPublisher : IEventPublisher
    {
        private readonly MongoDbContext _mongoDb;
        private readonly MongoSessionContext _sessionContext;
        public OutboxEventPublisher(MongoDbContext mongoDb, MongoSessionContext sessionContext)
        {
            _mongoDb = mongoDb;
            _sessionContext = sessionContext;
        }

        public async Task Publish<T>(T @Event, CancellationToken cancellationToken) where T : class
        {
            var outboxMessage = new OutboxMessage(typeof(T).AssemblyQualifiedName!,JsonSerializer.Serialize(Event));
            await _mongoDb.OutboxMessages.InsertOneAsync(_sessionContext.Session, outboxMessage, cancellationToken: cancellationToken);
        }
    }
}
