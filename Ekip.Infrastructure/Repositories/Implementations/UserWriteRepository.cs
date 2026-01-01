using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Infrastructure.Persistence;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class UserWriteRepository : IUserWriteRepository
    {
        private readonly MongoDbContext _mongoDb;
        public UserWriteRepository(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }
        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _mongoDb.Users.InsertOneAsync(user,cancellationToken:cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _mongoDb.Users.Find(x => email.Equals(x.Email)).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            return await _mongoDb.Users.Find(x => userName.Equals(x.UserName)).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
