
using Microsoft.Extensions.DependencyInjection;
using Application.Modules.User.CreateUser;

namespace Application;

public static class DependencyInyection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUSerHandler>();
        
        return services;
    }
}