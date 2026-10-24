var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Ekip>("ekip");

builder.Build().Run();
