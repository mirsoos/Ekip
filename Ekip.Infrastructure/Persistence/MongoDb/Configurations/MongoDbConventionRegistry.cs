using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.Requests.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations
{
    public static class MongoDbConventionRegistry
    {
        private static bool _isConfigured = false;

        public static void Configure()
        {
            if (_isConfigured) return;

            var conventionPack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true),
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreIfNullConvention(true),
                new ImmutableTypeClassMapConvention() 
            };

            ConventionRegistry.Register(
                "EkipConventions",
                conventionPack,
                t => t.Namespace != null && t.Namespace.StartsWith("Ekip.Domain"));

            _isConfigured = true;
        }
    }
}
