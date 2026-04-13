
using Microsoft.Extensions.DependencyInjection;
using Application.Modules.User.CreateUser;
using Application.Modules.Auth.Login;

namespace Application;

public static class DependencyInyection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<LoginHandler>();
        return services;
    }
}