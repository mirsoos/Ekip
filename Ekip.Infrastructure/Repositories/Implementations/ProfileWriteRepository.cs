using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ProfileWriteRepository : IProfileWriteRepository
    {
        private readonly MongoDbContext _mongoDb;
        public ProfileWriteRepository(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }
        public async Task<Profile> AddAsync(Profile profile, CancellationToken cancellationToken)
        {
            await _mongoDb.Profiles.InsertOneAsync(profile,cancellationToken:cancellationToken);
            return profile;
        }

        public async Task<bool> DoesProfileExistForUserAsync(Guid userRef, CancellationToken cancellationToken)
        {
            var profileExist = await _mongoDb.Profiles.CountDocumentsAsync(x=>x.UserRef == userRef,cancellationToken:cancellationToken);
            return profileExist > 0;
        }

        public async Task<Profile> GetByIdAsync(Guid profileRef, CancellationToken cancellationToken)
        {
             return await _mongoDb.Profiles.Find(x=>x.Id == profileRef).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Profile> UpdateAsync(Profile profile, CancellationToken cancellationToken)
        {
            var profileUpdated = await _mongoDb.Profiles.ReplaceOneAsync(x=> x.Id == profile.Id,profile,cancellationToken:cancellationToken);
            return profile;
        }
    }
}
