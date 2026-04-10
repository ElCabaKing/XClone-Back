using Infrastructure.Constants;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Exceptions;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
    IConfiguration configuration)
    {
        


        //External services configuration
        ConfigDatabase(services, configuration);
        
        return services;
    }

    public static void ConfigDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<XDbContext>(options =>
        options.UseSqlServer(
            configuration[ConfigurationConstants.ConnectionString] ?? throw new BadConfigurationException("Connection string is not configured.")
        ));
    }
}