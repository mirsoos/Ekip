using MongoDB.Bson.Serialization;
using Ekip.Domain.Entities.Chat.Entites;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Ekip.Domain.Enums.Chat.Enums;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations
{
    public class MessageConfiguration : IEntityConfiguration
    {
        private static bool _isConfigured = false;
        public void Configure()
        {
            if (_isConfigured)
                return;
            if (BsonClassMap.IsClassMapRegistered(typeof(Message)))
            {
                BsonClassMap.RegisterClassMap<Message>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(m => m.ChatRoomRef).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapMember(m => m.SentAt).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                    cm.MapMember(m => m.SenderRef).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapMember(m => m.ReplyToMessageRef).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapMember(m => m.RowVersion).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapMember(m => m.SeenBy).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapMember(m => m.Type).SetSerializer(new EnumSerializer<MessageType>(BsonType.String));
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
