using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Service;

public static class UserServiceServiceRegistration
{
    public static IServiceCollection AddUserServicesServices(
        this IServiceCollection services
        )
    {
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
    
}
