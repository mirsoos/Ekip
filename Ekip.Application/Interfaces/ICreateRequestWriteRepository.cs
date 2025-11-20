using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Application.Interfaces
{
    public interface ICreateRequestWriteRepository
    {
        Task<Request> AddRequestAsync(Request request,CancellationToken cancellationToken);
    }
}
