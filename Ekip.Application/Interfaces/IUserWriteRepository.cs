using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.Identity.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IUserWriteRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByUserNameAsync(string userName , CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetUserByUserNameOrEmailAsync(string userName , string email , CancellationToken cancellationToken);
        Task<User?> GetByUserIdAsync(Guid userRef,CancellationToken cancellationToken);
        Task<User> UpdateAsync(User user,CancellationToken cancellationToken);
        Task UpdateFaceVerificationStatusAsync(Guid userRef, Guid ReferenceId, string permanentUrl, string provider, CancellationToken cancellationToken);
        Task UpdateScoreAsync(Guid userRef, int scoreGiven, CancellationToken cancellationToken);

    }
}
