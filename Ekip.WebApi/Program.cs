using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ekip.Infrastructure.Configurations;
using Ekip.Infrastructure.Persistence;
using Ekip.Infrastructure.Repositories;
using Ekip.Infrastructure.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------
// 1️⃣ Configuration + Infrastructure
// -------------------------------------------------

// DI کامل Infrastructure (Postgres Read + Mongo Write + Services)
builder.Services.AddInfrastructure(builder.Configuration);

// -------------------------------------------------
// 2️⃣ Controllers & Swagger
// -------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------------------------------------
// 3️⃣ Middlewares / Pipeline
// -------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Swagger UI برای تست API در محیط توسعه
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ekip API V1");
        options.RoutePrefix = string.Empty; // نمایش Swagger در root
    });
}

// اجباری: HTTPS Redirection
app.UseHttpsRedirection();

// Authorization (JWT یا هر Policy که بعدا تعریف می‌کنیم)
app.UseAuthorization();

// Map Controllerها
app.MapControllers();

// Run برنامه
app.Run();
