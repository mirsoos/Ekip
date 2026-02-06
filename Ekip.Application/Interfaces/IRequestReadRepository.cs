using Ekip.Application.DTOs.Request;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Interfaces
{
    public interface IRequestReadRepository
    {
        Task<RequestReadModel> AddRequestAsync(RequestReadModel requestReadModel, CancellationToken cancellationToken);
        Task<RequestDetailsDto> GetRequestByIdAsync(Guid requestRef,CancellationToken cancellationToken);
        Task AddAssignmentAsync(RequestAssignmentReadModel requestAssignmentReadModel, CancellationToken cancellationToken);
        Task UpdateAsync(Guid requestRef,RequestStatus status,CancellationToken cancellationToken);
    }
}
