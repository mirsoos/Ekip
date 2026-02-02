using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations
{
    public class RequestConfiguration
    {
        private static bool _isConfigured = false;

        public void Configure()
        {
            if (_isConfigured) return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Request)))
            {
                BsonClassMap.RegisterClassMap<Request>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(r => r.Creator).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapMember(r => r.MemberType).SetSerializer(new EnumSerializer<MemberType>(BsonType.String));
                    cm.MapMember(r => r.RepeatType).SetSerializer(new EnumSerializer<RequestRepeatType>(BsonType.String));
                    cm.MapMember(r => r.RequestType).SetSerializer(new EnumSerializer<RequestType>(BsonType.String));
                    cm.MapMember(r => r.Status).SetSerializer(new EnumSerializer<RequestStatus>(BsonType.String));                    
                    cm.MapMember(r => r.CreateDate).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                    cm.MapMember(r => r.RequestDateTime).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                    cm.MapMember(r => r.RequestForbidDateTime).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));

                    cm.SetIgnoreExtraElements(true);
                });
            }

            ConfigureRequestAssignment();

            ConfigureRequestFilter();

            _isConfigured = true;
        }
        private static void ConfigureRequestAssignment()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(RequestAssignment)))
            {
                BsonClassMap.RegisterClassMap<RequestAssignment>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(false);
                    cm.MapMember(ra => ra.Status).SetSerializer(new EnumSerializer<AssignmentStatus>(BsonType.String));
                    cm.MapMember(ra => ra.ActionDate).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                    cm.MapMember(ra => ra.CreateDate).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                    cm.MapMember(ra => ra.SenderRef).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        private static void ConfigureRequestFilter()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(RequestFilter)))
            {
                BsonClassMap.RegisterClassMap<RequestFilter>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(false);
                    cm.MapMember(ra => ra.Kind).SetSerializer(new EnumSerializer<RequestFilterKind>(BsonType.String));
                    cm.MapMember(ra => ra.Type).SetSerializer(new EnumSerializer<RequestFilterType>(BsonType.String));

                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

    }
}
