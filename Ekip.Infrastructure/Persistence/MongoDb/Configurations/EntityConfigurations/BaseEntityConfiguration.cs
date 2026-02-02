using Ekip.Domain.Entities.Base.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations
{
    public static class BaseEntityConfiguration
    {
        public static void Configure()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
            {
                BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(be => be.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapMember(be => be.CreateDate).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                    cm.MapMember(be => be.IsDeleted).SetElementName("IsDeleted");
                    cm.SetIgnoreExtraElements(true);
                    cm.SetIsRootClass(true);
                });
            }
        }
    }
}
