using Microsoft.Extensions.DependencyInjection;

namespace QuickForm.Modules.Users.Jobs;

public static class JobServiceRegistration
{
    public static IServiceCollection AddUserJobServices(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        services.ConfigureOptions<ConfigureProcessInboxJob>();
        return services;
    }
}
