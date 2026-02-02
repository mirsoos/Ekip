using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Infrastructure.Configurations;
using Ekip.Infrastructure.Persistence.MongoDb.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Persistence.MongoDb.Contexts
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        static MongoDbContext()
        {
            MongoDbConfiguration.ConfigureExplicit();
        }

        public MongoDbContext(IMongoClient client, IOptions<InfrastructureSettings> options)
        {
            var dbName = options.Value.MongoDatabaseName;
            _database = client.GetDatabase(dbName);

            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            var requestsCollection = Requests;
            var indexKeys = Builders<Request>.IndexKeys
                .Ascending(r => r.CreateDate)
                .Ascending(r => r.Status);
            requestsCollection.Indexes.CreateOne(new CreateIndexModel<Request>(indexKeys));

            // Add other indexes as needed
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Profile> Profiles => _database.GetCollection<Profile>("Profiles");
        public IMongoCollection<Request> Requests => _database.GetCollection<Request>("Requests");
        public IMongoCollection<ChatRoom> ChatRooms => _database.GetCollection<ChatRoom>("ChatRooms");
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
        
    }
}
