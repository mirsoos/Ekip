using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Identity.Enums;

namespace Ekip.Application.Interfaces
{
    public interface IProfileReadRepository
    {
        Task<ProfileDto> GetUserDetailsByIdAsync(Guid userRef, CancellationToken cancellationToken);
        Task<ProfileDto> GetUserByIdAsync(Guid userRef, CancellationToken cancellationToken);
        Task AddProfileAsync(ProfileReadModel user, CancellationToken cancellationToken);
        Task UpdateAvatarAsync(Guid userRef , string avatarUrl , CancellationToken cancellationToken);
        Task UpdateFaceVerificationStatusAsync(Guid userRef , VerificationLevel verificationLevel , CancellationToken cancellationToken);

    }
}
