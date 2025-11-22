using Ekip.Domain.Entities.Identity.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IUserReadRepository
    {
        Task<User> GetByUserNameAsync(string userName,CancellationToken cancellationToken);
        Task<User> GetByIdAsync(int userId,CancellationToken cancellationToken);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User> GetByUserNameOrEmailAsync(string userName , string email,CancellationToken cancellationToken);
    }
}
