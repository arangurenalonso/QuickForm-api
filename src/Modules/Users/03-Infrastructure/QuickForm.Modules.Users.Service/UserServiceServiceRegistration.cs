using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Application;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Service;

public static class UserServiceServiceRegistration
{
    public static IServiceCollection AddUserServicesServices(
        this IServiceCollection services
        )
    {
        services.AddScoped<IAuthActionTokenHashingService, AuthActionTokenHashingService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthActionEmailService, AuthActionEmailService>();
        return services;
    }
    
}
