using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoClient client, IOptions<InfrastructureSettings> options)
        {
            var dbName = options.Value.MongoDatabaseName;
            _database = client.GetDatabase(dbName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Profile> Profiles => _database.GetCollection<Profile>("Profiles");
        public IMongoCollection<Request> Requests => _database.GetCollection<Request>("Requests");
        public IMongoCollection<RequestAssignment> RequestAssignments => _database.GetCollection<RequestAssignment>("JoinRequests");
        public IMongoCollection<ChatRoom> ChatRooms => _database.GetCollection<ChatRoom>("ChatRooms");
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
        
    }
}
