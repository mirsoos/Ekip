
using Ekip.Application.DTOs.Request;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IRequestReadRepository
    {
        Task<RequestReadModel> AddRequestAsync(RequestReadModel requestReadModel, CancellationToken cancellationToken);
        Task<RequestDetailsDto> GetRequestByIdAsync(Guid requestRef,CancellationToken cancellationToken);
        Task AddAssignmentAsync(RequestAssignmentReadModel requestAssignmentReadModel, CancellationToken cancellationToken);
    }
}
