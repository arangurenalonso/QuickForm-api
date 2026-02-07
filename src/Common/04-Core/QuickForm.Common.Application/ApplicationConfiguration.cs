using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace QuickForm.Common.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Assembly[] moduleApplicationAssemblies,
        List<Action<MediatRServiceConfiguration>>? configures = null)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(moduleApplicationAssemblies);

            config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
            //config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>))
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            if (configures != null)
            {
                foreach (var configure in configures)
                {
                    configure(config);
                }
            }
        });

        services.AddValidatorsFromAssemblies(moduleApplicationAssemblies, includeInternalTypes: true);

        return services;
    }
}
