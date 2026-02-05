using Ekip.Application.DTOs.User;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IUserReadRepository
    {
        Task<UserReadModel> GetByUserNameAsync(string userName,CancellationToken cancellationToken);
        Task<UserReadModel> GetByIdAsync(Guid userId,CancellationToken cancellationToken);
        Task<UserReadModel> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<UserReadModel> GetByUserNameOrEmailAsync(string userName , string email,CancellationToken cancellationToken);
        Task<ProfileReadModel> GetProfileByIdAsync(Guid profileRef,CancellationToken cancellationToken);
        Task<UserWithProfileDto> GetUserWithProfileByEmailOrUserNameAsync(string? userName,string? email,CancellationToken cancellationToken);
        Task AddUserAsync(UserReadModel user, CancellationToken cancellationToken);
    }
}
