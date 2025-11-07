using Ekip.Domain.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IUserWriteRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
    }
}
