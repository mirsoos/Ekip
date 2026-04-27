using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MongoDB.Bson;
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
        public async Task UpdateFaceVerificationStatusAsync(Guid profileRef, Guid referenceId, string permanentUrl,string provider, CancellationToken ct)
        {
            var filter = Builders<Profile>.Filter.Eq(p => p.Id, profileRef);

            var profile = await _mongoDb.Profiles.Find(filter).FirstOrDefaultAsync(ct);

            if (profile == null)
                throw new Exception("Profile not Found.");

            profile.UpdateVerificationPhoto(referenceId,provider, permanentUrl);

            await _mongoDb.Profiles.ReplaceOneAsync(filter, profile, cancellationToken: ct);
        }

        public async Task UpdateScoreAsync(Guid userRef, int scoreGiven, CancellationToken cancellationToken)
        {
            var collectionName = _mongoDb.Profiles.CollectionNamespace.CollectionName;
            var database = _mongoDb.Profiles.Database;

            var query = new BsonDocument();
            query.Add(new BsonElement("UserRef", BsonValue.Create(userRef)));

            var stage1 = new BsonDocument();
            stage1.Add(new BsonElement("$set",
                new BsonDocument
                {
                    { "TotalScoreSum", new BsonDocument("$add", new BsonArray { "$TotalScoreSum", scoreGiven }) },
                    { "TotalScoreCount", new BsonDocument("$add", new BsonArray { "$TotalScoreCount", 1 }) }
                }));

            var stage2 = new BsonDocument();
            stage2.Add(new BsonElement("$set",
                new BsonDocument
                {
                    { "Score", new BsonDocument("$divide", new BsonArray { "$TotalScoreSum", "$TotalScoreCount" }) },
                    { "RowVersion", BsonValue.Create(Guid.NewGuid()) }
                }));

            var updateStages = new BsonArray { stage1, stage2 };

            var update = new BsonDocument();
            update.Add(new BsonElement("q", query));
            update.Add(new BsonElement("u", updateStages));
            update.Add(new BsonElement("multi", false));

            var updatesArray = new BsonArray { update };

            var command = new BsonDocument();
            command.Add(new BsonElement("update", collectionName));
            command.Add(new BsonElement("updates", updatesArray));

            await database.RunCommandAsync<BsonDocument>(command, cancellationToken:cancellationToken);
        }

    }
}
