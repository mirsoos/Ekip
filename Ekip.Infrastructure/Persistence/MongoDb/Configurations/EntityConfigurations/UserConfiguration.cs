using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations
{
    public class UserConfiguration : IEntityConfiguration
    {
        private static bool _isConfigured = false;

        public void Configure()
        {
            if (_isConfigured) return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                BsonClassMap.RegisterClassMap<User>(cm =>
                {
                    cm.AutoMap();
                    cm.MapField("_userCredentials").SetElementName("UserCredentials");
                    cm.MapMember(r => r.Gender).SetSerializer(new EnumSerializer<GenderType>(BsonType.String));
                    cm.SetIgnoreExtraElements(true);

                });
            }

            ConfigureProfile();
            ConfigureEmail();
            ConfigureUserCredential();

            _isConfigured = true;
        }

        private static void ConfigureUserCredential()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(UserCredential)))
            {
                BsonClassMap.RegisterClassMap<UserCredential>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(u=>u.AuthenticationType).SetSerializer(new EnumSerializer<AuthenticationType>(BsonType.String));

                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
        private static void ConfigureProfile()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Profile)))
            {
                BsonClassMap.RegisterClassMap<Profile>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(u=>u.VerificationLevel).SetSerializer(new EnumSerializer<VerificationLevel>(BsonType.String));
                    cm.MapField("_userContacts").SetElementName("UserContacts");
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
        private static void ConfigureEmail()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Email)))
            {
                BsonClassMap.RegisterClassMap<Email>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(e => e.Value).SetElementName("Value");
                    cm.MapMember(e => e.Domain).SetElementName("Domain");
                    cm.MapMember(e => e.Normalized).SetElementName("Normalized");
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
