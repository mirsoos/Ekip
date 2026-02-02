using Ekip.Infrastructure.Persistence.MongoDb.Configurations.EntityConfigurations;
using System.Reflection;

namespace Ekip.Infrastructure.Persistence.MongoDb.Configurations
{
    public static class MongoDbConfiguration
    {
        private static bool _isConfigured = false;
        private static readonly object _lock = new object();

        public static void Configure()
        {
            lock (_lock)
            {
                if (_isConfigured) return;

                MongoDbConventionRegistry.Configure();

                BaseEntityConfiguration.Configure();

                ConfigureAllEntityConfigurations();

                _isConfigured = true;
            }
        }

        private static void ConfigureAllEntityConfigurations()
        {
            var configurationTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEntityConfiguration).IsAssignableFrom(t)
                         && t.IsClass
                         && !t.IsAbstract
                         && t != typeof(BaseEntityConfiguration));

            foreach (var type in configurationTypes)
            {
                var configuration = (IEntityConfiguration)Activator.CreateInstance(type);
                configuration.Configure();
            }
        }

        public static void ConfigureExplicit()
        {
            lock (_lock)
            {
                if (_isConfigured) return;

                MongoDbConventionRegistry.Configure();
                BaseEntityConfiguration.Configure();

                new RequestConfiguration().Configure();
                new UserConfiguration().Configure();
                new ProfileConfiguration().Configure();
                new ChatRoomConfiguration().Configure();
                new MessageConfiguration().Configure();

                _isConfigured = true;
            }
        }
    }
}
