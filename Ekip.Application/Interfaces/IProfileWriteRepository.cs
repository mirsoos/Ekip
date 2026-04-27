    using Ekip.Domain.Entities.Identity.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IProfileWriteRepository
    {
        Task <Profile> AddAsync(Profile profile, CancellationToken cancellationToken);
        Task <Profile> UpdateAsync(Profile profile, CancellationToken cancellationToken);
        Task<Profile> GetByIdAsync(Guid profileRef, CancellationToken cancellationToken);
        Task<bool> DoesProfileExistForUserAsync(Guid userRef, CancellationToken cancellationToken);
        Task UpdateScoreAsync(Guid userRef , int scoreGiven , CancellationToken cancellationToken);
        Task UpdateFaceVerificationStatusAsync(Guid profileRef,Guid ReferenceId ,string permanentUrl,string provider,CancellationToken cancellationToken);
    }
}
