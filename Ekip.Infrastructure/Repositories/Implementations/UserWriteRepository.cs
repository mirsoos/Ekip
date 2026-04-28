using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Exceptions;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MassTransit;
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
            try
            {
                await _mongoDb.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new DuplicateKeyException("Username or Email already exists.");
            }
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _mongoDb.Users.Find(x => email.Equals(x.Email)).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByProfileIdAsync(Guid profileRef, CancellationToken cancellationToken)
        {
            return await _mongoDb.Users.Find(x=> profileRef.Equals(x.ProfileRef)).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            return await _mongoDb.Users.Find(x => userName.Equals(x.UserName)).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User> GetUserByUserNameOrEmailAsync(string userName, string email, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(x=>x.UserName ,userName),
                Builders<User>.Filter.Eq(x=>x.Email.Value ,email));
            return await _mongoDb.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);

        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Eq(x=>x.Id ,user.Id);
            await _mongoDb.Users.ReplaceOneAsync(filter,user,cancellationToken:cancellationToken);
            return user;
        }
    }
}
