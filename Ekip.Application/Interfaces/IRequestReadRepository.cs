
using Ekip.Application.DTOs.Request;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IRequestReadRepository
    {
        Task<RequestDetailsDto> GetRequestByIdAsync(long requestRef,CancellationToken cancellationToken);
        Task<RequestReadModel> AddRequestAsync(RequestReadModel readModels, CancellationToken cancellationToken);
    }
}
