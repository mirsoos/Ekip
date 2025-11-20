using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.Identity.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IProfileWriteRepository
    {
        Task AddAsync(Profile profile, CancellationToken cancellationToken);
        Task UpdateAsync(Profile profile, CancellationToken cancellationToken);
        Task<Profile> GetByIdAsync(long profileRef, CancellationToken cancellationToken);
    }
}
