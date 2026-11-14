using Ekip.Application.Interfaces;
using Ekip.Infrastructure.Caching;
using Ekip.Infrastructure.Caching.Redis;
using Ekip.Infrastructure.Security.Password;
using Ekip.Infrastructure.Security.ReferralCode;
using Ekip.Persistence.Context;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ekip.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<IIdempotencyService, RedisIdempotencyService>();
        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();
        services.AddSingleton<IReferralCodeGenerator, ReferralCodeGenerator>();
        var rabbitConnectionString = configuration.GetConnectionString("rabbitmq")
            ?? throw new InvalidOperationException("Connection string 'rabbitmq' was not found.");

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers(typeof(Application.DependencyInjection).Assembly);

            x.AddEntityFrameworkOutbox<EkipDbContext>(o =>
            {
                o.UsePostgres();
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UseBusOutbox();
            });

            x.AddConfigureEndpointsCallback((context, _, cfg) =>
            {
                cfg.UseEntityFrameworkOutbox<EkipDbContext>(context);
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitConnectionString);
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}