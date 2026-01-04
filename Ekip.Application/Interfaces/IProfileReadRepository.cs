using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IProfileReadRepository
    {
        Task<ProfileDto> GetProfileDetailsByIdAsync(Guid profileRef, CancellationToken cancellationToken);
        Task<ProfileDto> GetProfileByIdAsync(Guid profileRef, CancellationToken cancellationToken);
        Task AddProfileAsync(ProfileReadModel profile, CancellationToken cancellationToken);
        Task UpdateAvatarAsync(Guid profileRef , string avatarUrl , CancellationToken cancellationToken);
    }
}
