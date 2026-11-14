using Ekip.Application.Interfaces;
using Ekip.Persistence.Context;
using Ekip.Persistence.Interceptors;
using Ekip.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pgvector.Npgsql;

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

        services.AddSingleton(sp =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());
            dataSourceBuilder.UseVector();
            dataSourceBuilder.UseNetTopologySuite();
            return dataSourceBuilder.Build();
        });

        services.AddDbContextPool<EkipDbContext>((sp, options) =>
        {
            var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
            var interceptor = sp.GetRequiredService<AuditableAndSoftDeleteInterceptor>();

            options.UseNpgsql(dataSource, npgsql =>
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

        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}