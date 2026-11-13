using Ekip.Persistence.Context;
using Ekip.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ekip.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuditableAndSoftDeleteInterceptor>();

        var connectionString = configuration.GetConnectionString("EkipDatabase")
            ?? throw new InvalidOperationException("Connection string 'EkipDatabase' was not found.");

        services.AddDbContextPool<EkipDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableAndSoftDeleteInterceptor>();

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(EkipDbContext).Assembly.FullName);
                npgsql.UseVector();
                npgsql.UseNetTopologySuite();
                npgsql.CommandTimeout(30);
            });

            options.UseSnakeCaseNamingConvention();
            options.AddInterceptors(interceptor);

#if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
        });

        return services;
    }
}