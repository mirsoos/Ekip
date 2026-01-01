using Ekip.Application.Features.Authentication.Consumers;
using Ekip.Application.Interfaces;
using Ekip.Infrastructure.Persistence;
using Ekip.Infrastructure.Repositories.Implementations;
using Ekip.Infrastructure.Repositories.Interfaces;
using Ekip.Infrastructure.Security;
using Ekip.Infrastructure.Services.Implementations;
using Ekip.Infrastructure.Services.Interfaces;
using MassTransit;
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

            var infraSettings = configuration.GetSection("InfrastructureSettings").Get<InfrastructureSettings>();

            services.AddMassTransit(busConfigurator =>
            {

                busConfigurator.SetKebabCaseEndpointNameFormatter();

                var appAssembly = typeof(UserCreatedConsumer).Assembly;
                busConfigurator.AddConsumers(appAssembly);

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(infraSettings.RabbitMqHost, "/", h =>
                    {
                        h.Username(infraSettings.RabbitMqUser);
                        h.Password(infraSettings.RabbitMqPassword);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });


            services.Configure<InfrastructureSettings>(
                configuration.GetSection("InfrastructureSettings")
            );


            services.AddDbContext<PostgresDbContext>(options =>
                    options.UseNpgsql(configuration.GetSection("InfrastructureSettings:PostgresConnection").Value));


            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoConnection = configuration.GetSection("InfrastructureSettings:MongoConnection").Value;
                return new MongoClient(mongoConnection);
            });


            services.AddScoped<MongoDbContext>();

            services.AddScoped<IMongoRepository, MongoRepository>();
            services.AddScoped<IRequestReadRepository, RequestReadRepository>();
            services.AddScoped<IRequestWriteRepository, RequestWriteRepository>();
            services.AddScoped<IChatRoomReadRepository, ChatRoomReadRepository>();
            services.AddScoped<IChatRoomWriteRepository, ChatRoomWriteRepository>();
            services.AddScoped<IMessageReadRepository, MessageReadRepository>();
            services.AddScoped<IMessageWriteRepository, MessageWriteRepository>();
            services.AddScoped<IProfileReadRepository, ProfileReadRepository>();
            services.AddScoped<IProfileWriteRepository, ProfileWriteRepository>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();


            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnection = configuration.GetSection("InfrastructureSettings:RedisConnection").Value;
                return ConnectionMultiplexer.Connect(redisConnection);
            });
            services.AddScoped<IRedisService, RedisService>();


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IMessageQueueService, RabbitMqService>();

            return services;
        }
    }
}
