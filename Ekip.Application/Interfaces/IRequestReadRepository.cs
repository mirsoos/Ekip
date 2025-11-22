
using Ekip.Application.DTOs.Request;

namespace Ekip.Application.Interfaces
{
    public interface IRequestReadRepository
    {
        Task<RequestDetailsDto> GetRequestByIdAsync(long requestRef,CancellationToken cancellationToken);
    }
}
