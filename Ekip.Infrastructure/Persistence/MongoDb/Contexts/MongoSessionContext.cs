using MongoDB.Driver;

namespace Ekip.Infrastructure.Persistence.MongoDb.Contexts
{
    public class MongoSessionContext
    {
        public IClientSessionHandle? Session { get; set; }
    }
}
