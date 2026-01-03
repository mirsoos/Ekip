using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IRequestWriteRepository
    {
        Task<Request> AddRequestAsync(Request request,CancellationToken cancellationToken);
        Task<Request?> GetRequestByIdAsync(Guid requestRef,CancellationToken cancellationToken);
        Task UpdateAsync(Request request, CancellationToken cancellationToken);
    }
}
