using Infrastructure.Constants;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Application.Interfaces;
using Infrastructure.Services;
using CloudinaryDotNet;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
    IConfiguration configuration)
    {
        //Repositories configuration
        services.AddScoped<IUserRepository, UserRepository>();
        


        //External services configuration
        services.AddSingleton(ConfigCloudinary(configuration));
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICloudStorage, CloudStorageService>();
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

    public static Cloudinary ConfigCloudinary(IConfiguration configuration)
    {
             var account = new Account(
    configuration[ConfigurationConstants.CloudinaryCloudName], 
    configuration[ConfigurationConstants.CloudinaryApiKey], 
    configuration[ConfigurationConstants.CloudinaryApiSecret]
);

    
        return new Cloudinary(account);;    
    }
}