using Ekip.Application.Interfaces;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public sealed class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoClient _client;
        private readonly MongoSessionContext _sessionContext;

        public MongoUnitOfWork(IMongoClient client, MongoSessionContext sessionContext)
        {
            _client = client;
            _sessionContext = sessionContext;
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken ct)
        {
            using var session = await _client.StartSessionAsync(cancellationToken: ct);
            _sessionContext.Session = session;

            session.StartTransaction();
            try
            {
                await operation(ct);
                await session.CommitTransactionAsync(ct);
            }
            catch
            {
                await session.AbortTransactionAsync(ct);
                throw;
            }
            finally
            {
                _sessionContext.Session = null;
            }
        }
    }
}
