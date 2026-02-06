using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class RequestWriteRepository : IRequestWriteRepository
    {
        private readonly MongoDbContext _mongoDb;

        public RequestWriteRepository(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<Request> AddRequestAsync(Request request, CancellationToken cancellationToken)
        {
            await _mongoDb.Requests.InsertOneAsync(request, cancellationToken: cancellationToken);
            return request;
        }

        public async Task<RequestAssignment> AssignRequest(Guid requestRef, RequestAssignment requestAssignment, CancellationToken cancellationToken)
        {
            var update = Builders<Request>.Update.Push("_assignments", requestAssignment);
            await _mongoDb.Requests.UpdateOneAsync(
                r => r.Id == requestRef,
                update,
                cancellationToken: cancellationToken
            );


            //if (result.MatchedCount == 0)
            //    throw new Exception($"Request {requestRef} Not Found.");

            return requestAssignment;
        }


        public async Task<Request> GetRequestByIdAsync(Guid requestRef, CancellationToken cancellationToken)
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
