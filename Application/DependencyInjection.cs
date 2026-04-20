
using Microsoft.Extensions.DependencyInjection;
using Application.Modules.Auth.Login;
using Application.Modules.Users.UpdateProfile;
using Application.Modules.Auth.Register;


namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //User Module

        services.AddScoped<UpdateProfileHandler>();
        //Auth Module
        services.AddScoped<LoginHandler>();
        services.AddScoped<RegisterHandler>();
        return services;
    }
}