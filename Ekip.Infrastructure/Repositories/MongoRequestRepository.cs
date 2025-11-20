using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Infrastructure.Repositories
{
    internal class MongoRequestRepository : ICreateRequestWriteRepository
    {
        public Task<Request> AddRequestAsync(Request request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
