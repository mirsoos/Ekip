using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class PostgreRequestRepositoy : IRequestReadRepository
    {
        public Task<RequestReadModel> AddRequestAsync(RequestReadModel requestReadModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanStartChatRoomAsync(long requestRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RequestDetailsDto> GetRequestByIdAsync(long requestRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RequestAssignmentReadModel> UpdateAsync(long requestRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
