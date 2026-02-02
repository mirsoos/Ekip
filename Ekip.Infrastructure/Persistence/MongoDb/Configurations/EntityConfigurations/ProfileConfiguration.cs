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
                    cm.MapMember(p=>p.UserContacts).SetSerializer(new EnumerableInterfaceImplementerSerializer<List<Guid>>(new GuidSerializer(GuidRepresentation.Standard)));
                    cm.MapMember(p=>p.CreateDate).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                    cm.MapMember(p=>p.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _isConfigured = true;
        }
    }
}
