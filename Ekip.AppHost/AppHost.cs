var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDockerfile("docker/postgres")
    .WithDataVolume("ekip-postgres-data")
    .WithPgWeb();

var ekipDb = postgres.AddDatabase("EkipDatabase");

var redis = builder.AddRedis("redis")
    .WithDataVolume("ekip-redis-data");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume("ekip-rabbitmq-data")
    .WithManagementPlugin();

builder.AddProject<Projects.Ekip_Api>("ekip-api")
    .WithReference(ekipDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(ekipDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq);

builder.Build().Run();