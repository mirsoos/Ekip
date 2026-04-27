using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.UserBehavior.Entities;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ScoreLedgerWriteRepository : IScoreLedgerWriteRepository
    {
        private readonly MongoDbContext _mongoDb;
        public ScoreLedgerWriteRepository(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task AddScoreAsync(ScoreLedger scoreLedger, CancellationToken cancellationToken)
        {
            await _mongoDb.ScoreLedgers.InsertOneAsync(scoreLedger, cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyCollection<ScoreLedger>> GetByProfileRefAsync(Guid profileRef, CancellationToken cancellationToken)
        {
            var filter = Builders<ScoreLedger>.Filter.Eq(x=>x.TargetUserProfileRef , profileRef);
            var result = await _mongoDb.ScoreLedgers.Find(filter).ToListAsync(cancellationToken);
            return result.AsReadOnly();
        }
    }
}
