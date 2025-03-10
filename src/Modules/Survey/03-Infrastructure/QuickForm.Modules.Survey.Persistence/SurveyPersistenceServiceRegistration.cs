using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain.Form;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public static class SurveyPersistenceServiceRegistration
{
    public static IServiceCollection AddUserPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("Survey:ConnectionStrings:Database").Value;


        services.AddScoped<AuditFieldsInterceptor>();
        services.AddScoped<InsertOutboxMessagesInterceptor>();
        services.AddScoped<AuditLogInterceptor>();


        services.AddDbContext<SurveyDbContext>((sp, options) =>
        {
            var interceptor1 = sp.GetRequiredService<AuditFieldsInterceptor>();
            var interceptor2 = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();
            var interceptor3 = sp.GetRequiredService<AuditLogInterceptor>();

            options.UseSqlServer(connectionString)
                   .AddInterceptors(interceptor1, interceptor2, interceptor3);
        });
        services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));


        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IFormRepository, FormRepository>();


        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SurveyDbContext>());
        services.AddSingleton<IDbConnectionFactory>(sp =>
                        new DbConnectionFactory(connectionString!));

        return services;
    }

}
