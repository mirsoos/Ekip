using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Infrastructure.Persistence;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class MongoRequestRepository : IRequestWriteRepository
    {

        private readonly MongoDbContext _mongoDb;

        public MongoRequestRepository(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<Request> AddRequestAsync(Request request, CancellationToken cancellationToken)
        {
            await _mongoDb.Requests.InsertOneAsync(request, cancellationToken: cancellationToken);
            return request;
        }

        public async Task<Request> GetRequestByIdAsync(long requestRef, CancellationToken cancellationToken)
        {
            var filters = Builders<Request>.Filter.Eq(r => r.Id, requestRef);
            
            return await _mongoDb.Requests.Find(filters).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync(Request request, CancellationToken cancellationToken)
        {
            await _mongoDb.Requests.ReplaceOneAsync(x => x.Id == request.Id, request, cancellationToken: cancellationToken);
        }
    }
}
