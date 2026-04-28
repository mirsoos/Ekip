using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Domain.Entities.UserBehavior.Entities;
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
            // ==================== User Indexes ====================
            var usersCollection = _database.GetCollection<User>("Users");
            try { usersCollection.Indexes.CreateOne(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.UserName), new CreateIndexOptions { Unique = true })); } catch { }
            try { usersCollection.Indexes.CreateOne(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email), new CreateIndexOptions { Unique = true })); } catch { }
            try { usersCollection.Indexes.CreateOne(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.PhoneNumber), new CreateIndexOptions { Unique = true })); } catch { }
            try { usersCollection.Indexes.CreateOne(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.ProfileRef))); } catch { }

            // ==================== Profile Indexes ====================
            var profilesCollection = _database.GetCollection<Profile>("Profiles");
            try { profilesCollection.Indexes.CreateOne(new CreateIndexModel<Profile>(Builders<Profile>.IndexKeys.Ascending(p => p.UserRef), new CreateIndexOptions { Unique = true })); } catch { }

            // ==================== Request Indexes ====================
            var requestsCollection = _database.GetCollection<Request>("Requests");
            try { requestsCollection.Indexes.CreateOne(new CreateIndexModel<Request>(Builders<Request>.IndexKeys.Ascending(r => r.Creator))); } catch { }
            try { requestsCollection.Indexes.CreateOne(new CreateIndexModel<Request>(Builders<Request>.IndexKeys.Ascending(r => r.Status))); } catch { }
            try { requestsCollection.Indexes.CreateOne(new CreateIndexModel<Request>(Builders<Request>.IndexKeys.Ascending(r => r.RequestDateTime))); } catch { }
            try { requestsCollection.Indexes.CreateOne(new CreateIndexModel<Request>(Builders<Request>.IndexKeys.Ascending(r => r.CreateDate))); } catch { }
            try
            {
                requestsCollection.Indexes.CreateOne(new CreateIndexModel<Request>(Builders<Request>.IndexKeys.Combine(
                Builders<Request>.IndexKeys.Ascending(r => r.Status),
                Builders<Request>.IndexKeys.Ascending(r => r.RequestDateTime)
            )));
            }
            catch { }

            // ==================== ChatRoom Indexes ====================
            var chatRoomsCollection = _database.GetCollection<ChatRoom>("ChatRooms");
            try { chatRoomsCollection.Indexes.CreateOne(new CreateIndexModel<ChatRoom>(Builders<ChatRoom>.IndexKeys.Ascending(c => c.Creator))); } catch { }
            try { chatRoomsCollection.Indexes.CreateOne(new CreateIndexModel<ChatRoom>(Builders<ChatRoom>.IndexKeys.Ascending(c => c.RequestRef))); } catch { }

            // ==================== Message Indexes ====================
            var messagesCollection = _database.GetCollection<Message>("Messages");
            try { messagesCollection.Indexes.CreateOne(new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Ascending(m => m.ChatRoomRef))); } catch { }
            try
            {
                messagesCollection.Indexes.CreateOne(new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Combine(
                Builders<Message>.IndexKeys.Ascending(m => m.ChatRoomRef),
                Builders<Message>.IndexKeys.Ascending(m => m.CreateDate)
            )));
            }
            catch { }
            try { messagesCollection.Indexes.CreateOne(new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Ascending(m => m.SenderRef))); } catch { }

            // ==================== ScoreLedger Indexes ====================
            var scoreLedgersCollection = _database.GetCollection<ScoreLedger>("ScoreLedgers");
            try { scoreLedgersCollection.Indexes.CreateOne(new CreateIndexModel<ScoreLedger>(Builders<ScoreLedger>.IndexKeys.Ascending(s => s.TargetUserProfileRef))); } catch { }
            try { scoreLedgersCollection.Indexes.CreateOne(new CreateIndexModel<ScoreLedger>(Builders<ScoreLedger>.IndexKeys.Ascending(s => s.SourceUserProfileRef))); } catch { }
            try { scoreLedgersCollection.Indexes.CreateOne(new CreateIndexModel<ScoreLedger>(Builders<ScoreLedger>.IndexKeys.Ascending(s => s.RequestRef))); } catch { }
            try
            {
                scoreLedgersCollection.Indexes.CreateOne(new CreateIndexModel<ScoreLedger>(Builders<ScoreLedger>.IndexKeys.Combine(
                Builders<ScoreLedger>.IndexKeys.Ascending(s => s.SourceUserProfileRef),
                Builders<ScoreLedger>.IndexKeys.Ascending(s => s.RequestRef)
            ), new CreateIndexOptions { Unique = true }));
            }
            catch { }
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Profile> Profiles => _database.GetCollection<Profile>("Profiles");
        public IMongoCollection<Request> Requests => _database.GetCollection<Request>("Requests");
        public IMongoCollection<ChatRoom> ChatRooms => _database.GetCollection<ChatRoom>("ChatRooms");
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
        public IMongoCollection<ScoreLedger> ScoreLedgers => _database.GetCollection<ScoreLedger>("ScoreLedgers");
        
    }
}
