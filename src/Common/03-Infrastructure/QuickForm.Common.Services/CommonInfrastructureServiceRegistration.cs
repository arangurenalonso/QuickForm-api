using System.Text;
using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
namespace QuickForm.Common.Infrastructure;

public static class CommonInfrastructureServiceRegistration
{
    public static IServiceCollection AddCommonServicesServices(
        this IServiceCollection services,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        IConfiguration configuration,
        string environment
        )
    {
        services.AddAuthenticationInternal(configuration,environment);
        services.AddAuthorizationInternal();

        services.AddSingleton(x => new BlobServiceClient(configuration["Common:AzureBlobStorage:ConnectionString"]));

        services.ConfigureOptions<ApplicationUrlsOptionsSetUp>();
        services.ConfigureOptions<AzureCommunicationEmailOptionsSetup>();

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IAzureCommunicationEmailService, AzureCommunicationEmailService>();
        services.AddScoped<ICommonOptionsProvider, CommonOptionsProvider>();

        services.AddJob();  
        services.AddIntegrationEvents(moduleConfigureConsumers);
        return services;
    }
    private static IServiceCollection AddJob(
       this IServiceCollection services
       )
    {
        services.AddQuartz();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
    private static IServiceCollection AddIntegrationEvents(
        this IServiceCollection services,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers
        )
    {
        services.TryAddSingleton<IEventBus, EventBus>();
        services.AddMassTransit(configure =>
        {
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
