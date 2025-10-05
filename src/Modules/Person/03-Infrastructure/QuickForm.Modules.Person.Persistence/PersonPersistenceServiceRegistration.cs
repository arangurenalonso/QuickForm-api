using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Infrastructure;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Person.Application;
using QuickForm.Modules.Person.Domain;

namespace QuickForm.Modules.Person.Persistence;
public static class SurveyPersistenceServiceRegistration
{
    public static IServiceCollection AddPersonPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("Person:ConnectionStrings:Database").Value;


        services.AddScoped<AuditFieldsInterceptor>();
        services.AddScoped<InsertOutboxMessagesInterceptor>();
        services.AddScoped<AuditLogInterceptor>();


        services.AddDbContext<PersonDbContext>((sp, options) =>
        {
            var interceptor1 = sp.GetRequiredService<AuditFieldsInterceptor>();
            var interceptor2 = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();
            var interceptor3 = sp.GetRequiredService<AuditLogInterceptor>();

            options.UseSqlServer(connectionString)
                   .AddInterceptors(interceptor1, interceptor2, interceptor3);
        });
        services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));




        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PersonDbContext>());
        services.AddSingleton<IDbConnectionFactory>(sp =>
                        new DbConnectionFactory(connectionString!));

        services.AddScoped(typeof(IGenericPersonRepository<,>), typeof(GenericPersonRepository<,>));
        return services;
    }

}

public class PersonPersistenceServiceRegistration
{

}
