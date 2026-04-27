using Ekip.Application.DTOs.User;

namespace Ekip.Application.Interfaces
{
    public interface IUserEkipReadRepository
    {
        Task<List<MyEkipDto>> GetEkipByProfileIdAsync(Guid profileRef,CancellationToken cancellationToken);
    }
}
