using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Application;
using QuickForm.Modules.Users.Persistence.Repositories;
using QuickForm.Modules.Users.Domain;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Common.Infrastructure.Authorization;

namespace QuickForm.Modules.Users.Persistence;
public static class UserPersistenceServiceRegistration
{
    public static IServiceCollection AddUserPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("Users:ConnectionStrings:Database").Value;


        services.AddScoped<AuditFieldsInterceptor>();
        services.AddScoped<InsertOutboxMessagesInterceptor>();
        services.AddScoped<AuditLogInterceptor>();


        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            var interceptor1 = sp.GetRequiredService<AuditFieldsInterceptor>();
            var interceptor2 = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();
            var interceptor3 = sp.GetRequiredService<AuditLogInterceptor>();

            options.UseSqlServer(connectionString)
                   .AddInterceptors(interceptor1, interceptor2, interceptor3);
        });
        services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));


        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserDapperRepository, UserDapperRepository>();
        services.AddScoped<IPermissionService, UserDapperRepository>();
        services.AddScoped<IAuthActionTokenRepository, AuthActionTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();



        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());
        services.AddSingleton<IDbConnectionFactory>(sp =>
                        new DbConnectionFactory(connectionString!));

        services.AddScoped(typeof(IGenericUserRepository<,>), typeof(GenericUserRepository<,>));
        services.AddScoped(typeof(IGenericUserMasterEntityRepository<>), typeof(GenericUserMasterEntityRepository<>));
        
        return services;
    }

}
