
using Application;
using AppWeb;
using AppWeb.Middlewares;
using DotNetEnv;
using Infrastructure;
using Infrastructure.Websockets;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi



Env.Load("../.env");

builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog();

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAppWeb(builder.Configuration);


builder.Services.AddControllers();


var app = builder.Build();

app.UseCors(
    policy =>
        policy.WithOrigins(
            "http://localhost:3000",
            "http://127.0.0.1:3000",
            "http://localhost:5173",
            "http://127.0.0.1:5173",
            "http://localhost:5500",
            "http://127.0.0.1:5500")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<NotificationHub>("/notifications");


app.Run();
