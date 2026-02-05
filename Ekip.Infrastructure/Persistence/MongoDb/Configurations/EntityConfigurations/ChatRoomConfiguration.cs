using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Enums.Chat.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations
{
    public class ChatRoomConfiguration : IEntityConfiguration
    {
        private static bool _isConfigured = false;

        public void Configure()
        {
            if (_isConfigured) return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(ChatRoom)))
            {
                BsonClassMap.RegisterClassMap<ChatRoom>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(c => c.ChatRoomType).SetSerializer(new EnumSerializer<ChatRoomType>(BsonType.String));
                    cm.MapMember(c => c.Creator).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapField("_participants").SetElementName("Participants");
                    cm.SetIgnoreExtraElements(true);
                });
            }
            _isConfigured = true;
        }
    }
}
