
using Ekip.Application.DTOs.Request;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IRequestReadRepository
    {
        Task<RequestDetailsDto> GetRequestByIdAsync(long requestRef,CancellationToken cancellationToken);
        Task<RequestReadModel> AddRequestAsync(RequestReadModel requestReadModel, CancellationToken cancellationToken);
        Task<RequestAssignmentReadModel> UpdateAsync(long requestRef, CancellationToken cancellationToken);
        Task<bool> CanStartChatRoomAsync(long requestRef,CancellationToken cancellationToken);
    }
}
