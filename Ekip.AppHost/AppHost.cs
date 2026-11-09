var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Ekip_Api>("ekip-api");

builder.Build().Run();
