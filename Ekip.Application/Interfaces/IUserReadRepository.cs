using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IUserReadRepository
    {
        Task<UserReadModel> GetByUserNameAsync(string userName,CancellationToken cancellationToken);
        Task<UserReadModel> GetByIdAsync(int userId,CancellationToken cancellationToken);
        Task<UserReadModel> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<UserReadModel> GetByUserNameOrEmailAsync(string userName , string email,CancellationToken cancellationToken);
        Task<UserReadModel> AddUserAsync(UserReadModel user, CancellationToken cancellationToken);
    }
}
