using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IProfileReadRepository
    {
        Task<ProfileDto> GetProfileDetailsByIdAsync(long profileRef, CancellationToken cancellationToken);
        Task<ProfileDto> GetProfileByIdAsync(long profileRef, CancellationToken cancellationToken);
        Task<ProfileReadModel> AddProfileAsync(ProfileReadModel profile, CancellationToken cancellationToken);
    }
}
