using Ekip.Api.gRPC;
using Ekip.Api.Middlewares;
using Ekip.Application;
using Ekip.Persistence;
using Ekip.Persistence.Context;
using Ekip.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.EnrichNpgsqlDbContext<EkipDbContext>(settings =>
{
    settings.DisableRetry = false;
    settings.DisableHealthChecks = false;
    settings.DisableTracing = false;
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<EkipDbContext>();
    await dbContext.Database.MigrateAsync();

    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();