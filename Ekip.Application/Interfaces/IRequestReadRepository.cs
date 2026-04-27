using Ekip.Application.DTOs.Request;
using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Interfaces
{
    public interface IRequestReadRepository
    {
        Task<RequestReadModel> AddRequestAsync(RequestReadModel requestReadModel, CancellationToken cancellationToken);
        Task<RequestDetailsDto> GetRequestByIdAsync(Guid requestRef,CancellationToken cancellationToken);
        Task AddAssignmentAsync(RequestAssignmentReadModel requestAssignmentReadModel, CancellationToken cancellationToken);
        Task UpdateAssignmentDecisionAsync(Guid assignmentRef , AssignmentStatus newStatus , CancellationToken cancellationToken);
        Task UpdateAsync(Guid requestRef,RequestStatus status,CancellationToken cancellationToken);
        Task<List<MyEkipDto>> GetEkipsByProfileIdAsync(Guid profileRef, CancellationToken cancellationToken);
        Task<List<PendingAssignmentsDto>> GetPendingAssignmentByProfileIdAsync(Guid profileRef, CancellationToken cancellationToken);

    }
}
