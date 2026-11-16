var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", "EkipDevPass123!", secret: true);
var redisPassword = builder.AddParameter("redis-password", "EkipDevPass123!", secret: true);
var rabbitUser = builder.AddParameter("rabbitmq-user", "ekip");
var rabbitPassword = builder.AddParameter("rabbitmq-password", "EkipDevPass123!", secret: true);

var postgres = builder.AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDockerfile("docker/postgres")
    .WithVolume("ekip-postgres-data", "/var/lib/postgresql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgWeb();

var ekipDb = postgres.AddDatabase("EkipDatabase");

#pragma warning disable ASPIRECERTIFICATES001
var redis = builder.AddRedis("redis", port: 6379, password: redisPassword)
    .WithoutHttpsCertificate()
    .WithDataVolume("ekip-redis-data")
    .WithLifetime(ContainerLifetime.Persistent);
#pragma warning restore ASPIRECERTIFICATES001

var rabbitmq = builder.AddRabbitMQ("rabbitmq", userName: rabbitUser, password: rabbitPassword, port: 5672)
    .WithDataVolume("ekip-rabbitmq-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

builder.AddProject<Projects.Ekip_Api>("ekip-api")
    .WithReference(ekipDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(ekipDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq);

builder.Build().Run();