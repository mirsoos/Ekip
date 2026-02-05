using Ekip.Domain.Entities.Identity.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations
{
    public class ProfileConfiguration : IEntityConfiguration
    {
        private static bool _isConfigured = false;

        public void Configure()
        {
            if (_isConfigured) return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Profile)))
            {
                BsonClassMap.RegisterClassMap<Profile>(cm =>
                {
                    cm.AutoMap();
                    cm.MapField("_userContacts").SetElementName("UserContacts");
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _isConfigured = true;
        }
    }
}
