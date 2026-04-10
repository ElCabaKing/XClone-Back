
using AppWeb.Middlewares;
using Infrastructure.Constants;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AppWeb;

public static class DependencyInjection
{
    public static IServiceCollection AddAppWeb(this IServiceCollection services,
    IConfiguration configuration)
    {

        services.AddScoped<ErrorHandlerMiddleware>();

        ConfigureLogger(services, configuration);


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

}