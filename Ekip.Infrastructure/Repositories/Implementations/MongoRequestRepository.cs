using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    internal class MongoRequestRepository : IRequestWriteRepository
    {
        public Task<bool> AddRequestAssignmentAsync(RequestAssignment requestAssignment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Request> AddRequestAsync(Request request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Request> GetRequestByIdAsync(long requestRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Request request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
