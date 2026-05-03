using Ekip.Domain.Entities.Outbox;
using Ekip.Domain.Enums.OutBox;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Diagnostics;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations
{
    public class OutBoxMessageConfiguration : IEntityConfiguration
    {
        private static bool _isConfigured = false;

        public void Configure()
        {
            if (_isConfigured) return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(OutboxMessage)))
            {
                BsonClassMap.RegisterClassMap<OutboxMessage>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(c => c.Status).SetSerializer(new EnumSerializer<Status>(BsonType.String));
                    cm.MapMember(c => c.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.SetIgnoreExtraElements(true);
                });
            }
            _isConfigured = true;
        }
    }
}
