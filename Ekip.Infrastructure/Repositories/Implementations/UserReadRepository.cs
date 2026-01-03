using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class UserReadRepository : IUserReadRepository
    {
        private readonly PostgresDbContext _postgresDb;

        public UserReadRepository(PostgresDbContext postgresDb)
        {
            _postgresDb = postgresDb;
        }
        public async Task AddUserAsync(UserReadModel user, CancellationToken cancellationToken)
        {
           await _postgresDb.UserReads.AddAsync(user,cancellationToken);
           await _postgresDb.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserReadModel> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _postgresDb.UserReads.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email,cancellationToken);
            return user;
        }

        public async Task<UserReadModel> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _postgresDb.UserReads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId,cancellationToken);
            return user;
        }

        public async Task<UserReadModel> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            var user = await _postgresDb.UserReads.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
            return user;
        }

        public async Task<UserReadModel> GetByUserNameOrEmailAsync(string userName, string email, CancellationToken cancellationToken)
        {
            var user = await _postgresDb.UserReads.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName || x.Email == email,cancellationToken);
            return user;
        }
    }
}
