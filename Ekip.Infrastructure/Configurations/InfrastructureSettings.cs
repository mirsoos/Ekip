
namespace Ekip.Infrastructure.Configurations
{
    public class InfrastructureSettings
    {
        public string RabbitMqHost { get; set; } = string.Empty;
        public string RabbitMqUser { get; set; } = string.Empty;
        public string RabbitMqPassword { get; set; } = string.Empty;

        public string EmailSenderAddress { get; set; } = string.Empty;
        public string EmailSenderPassword { get; set; } = string.Empty;
        public string EmailSmtpHost { get; set; } = string.Empty;
        public int EmailSmtpPort { get; set; }

        public string JwtSecret { get; set; } = string.Empty;
        public int JwtExpiryMinutes { get; set; }

        public string RedisConnection { get; set; } = string.Empty;

        public string MongoConnection { get; set; } = string.Empty;
        public string MongoDatabaseName { get; set; } = string.Empty;


        public string PostgresConnection { get; set; } = string.Empty;
    }
}
