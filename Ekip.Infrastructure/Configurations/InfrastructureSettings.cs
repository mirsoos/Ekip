
namespace Ekip.Infrastructure.Configurations
{
    public class InfrastructureSettings
    {
        // RabbitMQ برای مدیریت پیام‌ها (چت async یا queue)
        public string RabbitMqHost { get; set; } = string.Empty;
        public string RabbitMqUser { get; set; } = string.Empty;
        public string RabbitMqPassword { get; set; } = string.Empty;

        // Email sender (مثلاً برای تایید ایمیل یا اطلاعیه‌ها)
        public string EmailSenderAddress { get; set; } = string.Empty;
        public string EmailSenderPassword { get; set; } = string.Empty;
        public string EmailSmtpHost { get; set; } = string.Empty;
        public int EmailSmtpPort { get; set; }

        // JWT Secret برای احراز هویت
        public string JwtSecret { get; set; } = string.Empty;
        public int JwtExpiryMinutes { get; set; }

        // Redis (مثلاً کش پیام‌ها یا session)
        public string RedisConnection { get; set; } = string.Empty;

        // MongoDB (Write DB)
        public string MongoConnection { get; set; } = string.Empty;
        public string MongoDatabaseName { get; set; } = string.Empty;


        // PostgreSQL (Read DB)
        public string PostgresConnection { get; set; } = string.Empty;
    }
}
