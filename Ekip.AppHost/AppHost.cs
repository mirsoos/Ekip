var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDockerfile("docker/postgres")
    .WithDataVolume("ekip-postgres-data")
    .WithPgWeb();

var ekipDb = postgres.AddDatabase("EkipDatabase");

builder.AddProject<Projects.Ekip_Api>("ekip-api")
    .WithReference(ekipDb)
    .WaitFor(ekipDb);

builder.Build().Run();