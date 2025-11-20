using Ekip.Application.DTOs.User;

namespace Ekip.Application.Interfaces
{
    public interface IProfileReadRepository
    {
        Task<ProfileDto> GetProfileDetailsByIdAsync(long profileRef, CancellationToken cancellationToken);
    }
}
