using Ekip.Domain.Entities.Identity.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IUserWriteRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User> GetByUserNameAsync(string userName , CancellationToken cancellationToken);
        Task<bool> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
