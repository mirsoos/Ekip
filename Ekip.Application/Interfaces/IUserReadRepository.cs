using Ekip.Domain.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IUserReadRepository
    {
        Task<User> GetByUserNameAsync(string userName);
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUserNameOrEmailAsync(string userName , string email);
    }
}
