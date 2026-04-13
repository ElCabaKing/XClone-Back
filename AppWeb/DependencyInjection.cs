
using System.Threading.RateLimiting;
using AppWeb.Middlewares;
using Infrastructure.Constants;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.Constants;

namespace AppWeb;

public static class DependencyInjection
{
    public static IServiceCollection AddAppWeb(this IServiceCollection services,
    IConfiguration configuration)
    {

        services.AddScoped<ErrorHandlerMiddleware>();

        ConfigureLogger(services, configuration);
        ConfigureRateLimit(services, configuration);


        return services;
    }

    private static void ConfigureLogger(IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.MongoDB(configuration[ConfigurationConstants.MongoConnectionString] ??
    throw new InvalidOperationException("MongoDB Connection String is not configured."), collectionName: "Logs")
    .CreateLogger();
    }
    private static void ConfigureRateLimit(IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("Fixed", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    Message = ResponseConstants.TOO_MANY_REQUESTS
                }, cancellationToken);
            };
        });
    }
}