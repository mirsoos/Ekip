using Ekip.Domain.Entities.Chat.Entites;
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

        public IMongoCollection<ChatRoom> ChatRooms => _database.GetCollection<ChatRoom>("ChatRooms");
        
    }
}
