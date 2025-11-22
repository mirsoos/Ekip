using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;

namespace Ekip.Infrastructure.Repositories
{
    public class PostgreRequestRepositoy : IRequestReadRepository
    {
        public Task<RequestDetailsDto> GetRequestByIdAsync(long requestRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
