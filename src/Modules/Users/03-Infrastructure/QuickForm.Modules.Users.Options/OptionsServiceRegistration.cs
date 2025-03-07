using Microsoft.Extensions.DependencyInjection;

namespace QuickForm.Modules.Users.Options;

public static class OptionsServiceRegistration
{
    public static IServiceCollection AddOptionsServices(this IServiceCollection services)
    {

        services.ConfigureOptions<OutboxOptionsSetup>();
        services.ConfigureOptions<InboxOptionsSetup>();
        return services;
    }
}
