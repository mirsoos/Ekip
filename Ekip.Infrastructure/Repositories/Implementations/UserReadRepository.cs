using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
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
            var user = await _postgresDb.UserReads.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName || x.Email == email);
            return user;
        }

        public async Task<ProfileReadModel> GetProfileByIdAsync(Guid profileRef, CancellationToken cancellationToken)
        {
            var profile = await _postgresDb.ProfileReads.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == profileRef,cancellationToken);
            return profile;
        }

        public async Task<UserWithProfileDto?> GetUserWithProfileByEmailOrUserNameAsync(string? userName,string? email, CancellationToken cancellationToken)
        {
            var result = await (from u in _postgresDb.UserReads
                                join p in _postgresDb.ProfileReads on u.ProfileRef equals p.Id
                                where (userName != null && u.UserName == userName) || (email != null && u.Email == email)
                                select new { User = u, Profile = p })
                                .AsNoTracking()
                                .FirstOrDefaultAsync(cancellationToken);

            if (result == null) return null;

            return new UserWithProfileDto
            {
                ProfileRef = result.Profile.Id,
                UserRef = result.User.Id,
                Email = result.User.Email,
                UserName = result.User.UserName,
                PhoneNumber = result.User.PhoneNumber,
                AvatarUrl = result.Profile.AvatarUrl,
            };
        }
    }
}
