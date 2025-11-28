using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Interfaces
{
    public interface IMongoRepository
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
