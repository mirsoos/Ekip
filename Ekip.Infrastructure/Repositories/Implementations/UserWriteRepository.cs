using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Exceptions;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MongoDB.Bson;
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
                throw new DuplicateKeyException("Username ,Email Or PhoneNumber already exists.");
            }
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _mongoDb.Users.Find(x => email.Equals(x.Email)).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByUserIdAsync(Guid userRef, CancellationToken cancellationToken)
        {
            return await _mongoDb.Users.Find(x=> userRef.Equals(x.Id)).FirstOrDefaultAsync(cancellationToken);
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
        public async Task UpdateFaceVerificationStatusAsync(Guid userRef, Guid referenceId, string permanentUrl, string provider, CancellationToken ct)
        {
            var filter = Builders<User>.Filter.Eq(p => p.Id, userRef);

            var user = await _mongoDb.Users.Find(filter).FirstOrDefaultAsync(ct);

            if (user == null)
                throw new Exception("User not Found.");

            user.Profile.UpdateVerificationPhoto(referenceId, provider, permanentUrl);

            await _mongoDb.Users.ReplaceOneAsync(filter, user, cancellationToken: ct);
        }
        public async Task UpdateScoreAsync(Guid userRef, int scoreGiven, CancellationToken cancellationToken)
        {
            var collectionName = _mongoDb.Users.CollectionNamespace.CollectionName;
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

            await database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);
        }
    }
}
