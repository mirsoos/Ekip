using Ekip.Application.DTOs.User;

namespace Ekip.Application.Interfaces.Queries.User;

public interface IUserQueries
{
    Task<UserProfileDto?> GetProfileByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}