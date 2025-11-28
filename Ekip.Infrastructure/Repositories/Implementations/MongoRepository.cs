using Ekip.Infrastructure.Persistence;
using Ekip.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class MongoRepository : IMongoRepository
    {
        private readonly MongoDbContext _context;

        public MongoRepository(MongoDbContext context)
        {
            _context = context;
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _context.GetType()
                           .GetProperty(name)
                           ?.GetValue(_context) as IMongoCollection<T>
                   ?? throw new Exception($"Collection {name} not found in MongoDbContext.");
        }
    }
}