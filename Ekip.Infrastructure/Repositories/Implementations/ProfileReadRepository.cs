using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ProfileReadRepository : IProfileReadRepository
    {
        public Task<ProfileReadModel> AddProfileAsync(ProfileReadModel profile, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileDto> GetProfileByIdAsync(long profileRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileDto> GetProfileDetailsByIdAsync(long profileRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
