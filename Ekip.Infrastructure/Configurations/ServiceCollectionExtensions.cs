using Ekip.Infrastructure.Persistence;
using Ekip.Infrastructure.Repositories.Implementations;
using Ekip.Infrastructure.Repositories.Interfaces;
using Ekip.Infrastructure.Services.Implementations;
using Ekip.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Ekip.Infrastructure.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // -----------------------------
            // 1️⃣ Options
            // -----------------------------
            services.Configure<InfrastructureSettings>(
                configuration.GetSection("InfrastructureSettings")
            );

            // -----------------------------
            // 2️⃣ PostgreSQL DbContext (Read)
            // -----------------------------
            services.AddDbContext<PostgresDbContext>(options =>
                    options.UseNpgsql(configuration.GetSection("InfrastructureSettings:PostgresConnection").Value));

            // -----------------------------
            // 3️⃣ MongoDB Client (Write)
            // -----------------------------
            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoConnection = configuration.GetSection("InfrastructureSettings:MongoConnection").Value;
                return new MongoClient(mongoConnection);
            });

            // MongoDbContext
            services.AddScoped<MongoDbContext>();

            // MongoRepository
            services.AddScoped<IMongoRepository, MongoRepository>();


            // -----------------------------
            // 4️⃣ Redis
            // -----------------------------
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnection = configuration.GetSection("InfrastructureSettings:RedisConnection").Value;
                return ConnectionMultiplexer.Connect(redisConnection);
            });
            services.AddScoped<IRedisService, RedisService>();

            // -----------------------------
            // 5️⃣ Repositories
            // -----------------------------
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            // سایر repository های اختصاصی اگر داری اضافه می‌شن

            // -----------------------------
            // 6️⃣ Services عمومی (Email, RabbitMQ)
            // -----------------------------
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMessageQueueService, RabbitMqService>();

            return services;
        }
    }
}
