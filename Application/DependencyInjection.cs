
using Microsoft.Extensions.DependencyInjection;
using Application.Modules.Auth.Login;
using Application.Modules.Users.CreateUser;
using Application.Modules.Users.UpdateUser;


namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //User Module
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<UpdateUserHandler>();  
        //Auth Module
        services.AddScoped<LoginHandler>();
        return services;
    }
}