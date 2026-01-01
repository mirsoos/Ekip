using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class UserReadRepository : IUserReadRepository
    {
        public Task<UserReadModel> AddUserAsync(UserReadModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserReadModel> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserReadModel> GetByIdAsync(int userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserReadModel> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserReadModel> GetByUserNameOrEmailAsync(string userName, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
