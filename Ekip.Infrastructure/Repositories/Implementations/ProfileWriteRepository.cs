using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ProfileWriteRepository : IProfileWriteRepository
    {
        public Task<Profile> AddAsync(Profile profile, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoesProfileExistForUserAsync(long userRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Profile> GetByIdAsync(long profileRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Profile> UpdateAsync(Profile profile, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
