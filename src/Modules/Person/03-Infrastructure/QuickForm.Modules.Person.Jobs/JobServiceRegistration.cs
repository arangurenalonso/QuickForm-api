using Microsoft.Extensions.DependencyInjection;

namespace QuickForm.Modules.Person.Jobs;

public static class JobServiceRegistration
{
    public static IServiceCollection AddPersonJobServices(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        services.ConfigureOptions<ConfigureProcessInboxJob>();
        return services;
    }
}
