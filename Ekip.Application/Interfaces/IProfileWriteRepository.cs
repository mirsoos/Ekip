using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IProfileWriteRepository
    {
        Task <Profile> AddAsync(Profile profile, CancellationToken cancellationToken);
        Task <Profile> UpdateAsync(Profile profile, CancellationToken cancellationToken);
        Task<Profile> GetByIdAsync(long profileRef, CancellationToken cancellationToken);
        Task<bool> DoesProfileExistForUserAsync(long userRef, CancellationToken cancellationToken);
    }
}
