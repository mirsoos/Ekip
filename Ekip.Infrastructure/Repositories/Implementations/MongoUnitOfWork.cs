using Ekip.Application.Interfaces;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public sealed class MongoUnitOfWork : IUnitOfWork
    {
        private IMongoClient _client;

        public MongoUnitOfWork(IMongoClient client) 
        {
            _client = client;
        }

        public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken)
        {
            using var session = await _client.StartSessionAsync(cancellationToken:cancellationToken);

            session.StartTransaction();

            try
            {
                await operation();
                await session.CommitTransactionAsync(cancellationToken:cancellationToken);
            }
            catch
            {
                await session.AbortTransactionAsync(cancellationToken:cancellationToken);
                throw;
            }
        }
    }
}
