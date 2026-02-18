using Ekip.Application;
using Ekip.Infrastructure.Configurations;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==== سرویس‌های اصلی اپلیکیشن ====
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ==== Swagger + JWT ====
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ekip API", Version = "v1" });

    // Security Definition برای JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT token into field. Example: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // Security Requirement روی تمام ریکوئست‌ها
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// ==== CORS برای SignalR ====
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "http://localhost:17177"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ==== Swagger UI در محیط Development ====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ekip API V1");
        c.RoutePrefix = string.Empty; // وقتی localhost بزنی مستقیم Swagger UI باز می‌شه
    });
}

// ==== Middleware ها ====
app.UseRouting();
app.UseCors("SignalRCors");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); // فایل‌های wwwroot سرو می‌شن

// ==== Controller ها و SignalR ====
app.MapControllers();
app.MapHub<Ekip.Infrastructure.Services.SignalR.ChatHub>("/chathub");

app.Run();
