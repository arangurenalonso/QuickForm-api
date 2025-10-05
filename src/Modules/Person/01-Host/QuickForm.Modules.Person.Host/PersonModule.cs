using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuickForm.Common.Application;
using QuickForm.Modules.Person.Persistence;

namespace QuickForm.Modules.Person.Host;
public static class PersonModule
{
    public static IServiceCollection AddPersonModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddPersonPersistenceServices(configuration);

        return services;
    }
}
