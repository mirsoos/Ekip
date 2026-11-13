using Ekip.ServiceDefaults;
using Ekip.Persistence;
using Ekip.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.EnrichNpgsqlDbContext<EkipDbContext>(settings =>
{
    settings.DisableRetry = false;
    settings.DisableHealthChecks = false;
    settings.DisableTracing = false;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<EkipDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();