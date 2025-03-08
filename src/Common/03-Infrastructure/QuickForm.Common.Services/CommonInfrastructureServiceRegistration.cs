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
    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration,string environment)
    {
        services.ConfigureOptions<JwtOptionsSetup>();

        JwtOptions jwtOptions = configuration.GetSection("Common:Jwt").Get<JwtOptions>() ?? new JwtOptions();
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {

                options.RequireHttpsMetadata = environment == AppConstants.EnvironmentProduction;
                /*
                 * Si está en true (por defecto):
                 *  - Requiere HTTPS para obtener los metadatos del Issuer (proveedor de identidad)
                 *  - Rechaza HTTP y lanza una excepción si el servidor de autenticación no usa https://.
                 * Si está en false:
                 *  - Permite HTTP para obtener metadatos, útil en entornos de desarrollo donde no hay HTTPS
                 * Cuándo ponerlo en false
                 *  - Entornos de desarrollo donde el servidor de autenticación aún no usa HTTPS
                 */
                // Asegurar HTTPS (ajustar si necesario)
                options.SaveToken = true;
                /*
                 * indica si el middleware de autenticación de JWT en ASP.NET Core debe almacenar el token recibido en el contexto de autenticación (HttpContext.User)
                 * Si está en false (valor por defecto)
                 *  - El token no se almacena automáticamente en HttpContext
                 *  - Si necesitas el token en otro lugar del código (por ejemplo, en un middleware personalizado), 
                 *    tendrías que obtenerlo de los encabezados de la solicitud manualmente
                 *  - Forma de acceder var token = await HttpContext.GetTokenAsync("access_token")
                 * Si está en true
                 *  - El token se guarda en HttpContext, y puedes acceder a él en cualquier parte del pipeline
                 *  - Forma de acceder var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "")
                 * .*/
                options.TokenValidationParameters = new()
                {
                    ValidAudience = jwtOptions.Audience,
                    ValidIssuer = jwtOptions.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                    /* 
                     * Define un margen de tiempo permitido para compensar posibles diferencias de reloj 
                     * entre el emisor del token (Identity Provider) y el servidor que lo valida
                     * Por defecto, el valor es 5 minutos.
                     * ClockSkew = TimeSpan.Zero => Elimina ese margen de tolerancia.
                     */
                };
            });

        services.AddHttpContextAccessor();

        services.TryAddSingleton<ICurrentUserService, CurrentUserService>();
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
