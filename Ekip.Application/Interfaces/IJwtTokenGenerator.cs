using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.ReadModels;

namespace Ekip.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(UserReadModel user);
        string GenerateToken(User user);
    }
}
