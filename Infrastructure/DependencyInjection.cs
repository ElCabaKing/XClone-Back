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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
    IConfiguration configuration)
    {
        //Repositories configuration
        services.AddScoped<IUserRepository, UserRepository>();



        //External services configuration
        services.AddSingleton(ConfigureCloudinary(configuration));
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICloudStorage, CloudStorageService>();
        ConfigureDatabase(services, configuration);
        services.Configure<JwtSettings>(
configuration.GetSection("Jwt"));
        ConfigureAuthentication(services, configuration);
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<XDbContext>(options =>
        options.UseSqlServer(
            configuration[ConfigurationConstants.ConnectionString] ?? throw new BadConfigurationException("Connection string is not configured.")
        ));
    }

    public static Cloudinary ConfigureCloudinary(IConfiguration configuration)
    {
        return new Cloudinary(new Account(
configuration[ConfigurationConstants.CloudinaryCloudName],
configuration[ConfigurationConstants.CloudinaryApiKey],
configuration[ConfigurationConstants.CloudinaryApiSecret]));
    }

    private static void ConfigureAuthentication(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = GetJwtSettings(configuration);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt.Key)
        );

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

    }
    private static JwtSettings GetJwtSettings(IConfiguration configuration)
    {
        return configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new BadConfigurationException(ServicesResponseConstants.JWT_CONFIG_ERROR);
    }
}