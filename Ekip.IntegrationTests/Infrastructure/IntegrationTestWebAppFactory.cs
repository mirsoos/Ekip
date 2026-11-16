using Ekip.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;
using Xunit;

namespace Ekip.IntegrationTests.Infrastructure;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestDbConnectionString =
        "Host=localhost;Port=5432;Database=ekip_integration_tests;Username=postgres;Password=EkipDevPass123!;Include Error Detail=true;";

    private const string RedisConnectionString =
        "localhost:6379,password=EkipDevPass123!";

    private const string RabbitMqConnectionString =
        "amqp://ekip:EkipDevPass123!@localhost:5672";

    public IntegrationTestWebAppFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("ConnectionStrings__EkipDatabase", TestDbConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__redis", RedisConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__rabbitmq", RabbitMqConnectionString);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    public async ValueTask InitializeAsync()
    {
        await InitializeDatabaseAsync();

        await ResetRabbitMqQueuesAsync();

        await ResetRedisDatabaseAsync();

        _ = Services;
    }

    private static async Task InitializeDatabaseAsync()
    {
        var optionsBuilder = new DbContextOptionsBuilder<EkipDbContext>();
        optionsBuilder.UseNpgsql(TestDbConnectionString, npgsqlOptions =>
        {
            npgsqlOptions.UseVector();
            npgsqlOptions.UseNetTopologySuite();
        });
        optionsBuilder.UseSnakeCaseNamingConvention();

        await using var dbContext = new EkipDbContext(optionsBuilder.Options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    private static async Task ResetRabbitMqQueuesAsync()
    {
        try
        {
            var factory = new ConnectionFactory { Uri = new Uri(RabbitMqConnectionString) };
            await using var connection = await factory.CreateConnectionAsync();

            await TryPurgeQueueAsync(connection, "user-registered");
            await TryPurgeQueueAsync(connection, "user-registered_error");
        }
        catch
        {
        }
    }

    private static async Task TryPurgeQueueAsync(IConnection connection, string queueName)
    {
        try
        {
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueuePurgeAsync(queueName);
        }
        catch
        {
        }
    }

    private static async Task ResetRedisDatabaseAsync()
    {
        try
        {
            var options = ConfigurationOptions.Parse(RedisConnectionString);
            options.AllowAdmin = true;

            await using var redis = await ConnectionMultiplexer.ConnectAsync(options);
            var endpoint = redis.GetEndPoints().FirstOrDefault();
            if (endpoint is not null)
            {
                var server = redis.GetServer(endpoint);
                await server.FlushDatabaseAsync();
            }
        }
        catch
        {
        }
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }
}