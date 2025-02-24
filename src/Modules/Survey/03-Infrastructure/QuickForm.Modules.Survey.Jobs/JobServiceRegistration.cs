using Microsoft.Extensions.DependencyInjection;

namespace QuickForm.Modules.Survey.Jobs;

public static class JobServiceRegistration
{
    public static IServiceCollection AddSurveyJobServices(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        services.ConfigureOptions<ConfigureProcessInboxJob>();
        return services;
    }
}
