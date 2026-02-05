using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid id ,string phoneNumber , string userName);
    }
}
