using Ekip.Application.Features.Authentication.Consumers;
using Ekip.Application.Interfaces;
using Ekip.Infrastructure.Persistence.MongoDb.Configurations;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
using Ekip.Infrastructure.Repositories.Implementations;
using Ekip.Infrastructure.Repositories.Interfaces;
using Ekip.Infrastructure.Security;
using Ekip.Infrastructure.Services.FaceAI;
using Ekip.Infrastructure.Services.Implementations;
using Ekip.Infrastructure.Services.Interfaces;
using Ekip.Infrastructure.Services.Redis.Implementations;
using Ekip.Infrastructure.Services.Redis.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Npgsql;
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

                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            MongoDbConventionRegistry.Configure();

            services.Configure<InfrastructureSettings>(
                configuration.GetSection("InfrastructureSettings")
            );

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(
                configuration.GetSection("InfrastructureSettings:PostgresConnection").Value);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(dataSource));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                          System.Text.Encoding.UTF8.GetBytes(infraSettings.JwtSecret)),
                      ValidateIssuer = false,
                      ValidateAudience = false,
                      ValidateLifetime = true
                  };

                  options.Events = new JwtBearerEvents
                  {
                      OnMessageReceived = context =>
                      {
                          var accessToken = context.Request.Query["access_token"];
                          var path = context.HttpContext.Request.Path;
                          if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                          {
                              context.Token = accessToken;
                          }
                          return Task.CompletedTask;
                      }
                  };
              });
            services.AddSignalR().AddStackExchangeRedis(infraSettings.RedisConnection);

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoConnection = configuration.GetSection("InfrastructureSettings:MongoConnection").Value;
                return new MongoClient(mongoConnection);
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(infraSettings.RedisConnection));

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
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IUserEkipReadRepository, UserEkipReadRepository>();
            services.AddScoped<IUserEkipUpdaterService, UserEkipUpdaterService>();

            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddSingleton<InsightFaceEngine>();
            services.AddScoped<IFaceVerificationService, InsightFaceVerificationService>();


            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IMessageQueueService, RabbitMqService>();

            return services;
        }
    }
}
