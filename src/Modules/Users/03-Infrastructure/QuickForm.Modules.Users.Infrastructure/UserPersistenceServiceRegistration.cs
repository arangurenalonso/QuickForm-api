using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Application;
using QuickForm.Modules.Users.Persistence.Repositories;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public static class UserPersistenceServiceRegistration
{
    public static IServiceCollection AddUserPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("Users:ConnectionStrings:Database").Value;


        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<InsertOutboxMessagesInterceptor>();
        

        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            var interceptor1 = sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>();
            var interceptor2 = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseSqlServer(connectionString)
                   .AddInterceptors(interceptor1, interceptor2);
        });
        services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));


        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserDapperRepository, UserDapperRepository>();
        services.AddScoped<IAuthActionTokenRepository, AuthActionTokenRepository>();
        


            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());
        services.AddSingleton<IDbConnectionFactory>(sp =>
                        new DbConnectionFactory(connectionString!));

        return services;
    }

}
