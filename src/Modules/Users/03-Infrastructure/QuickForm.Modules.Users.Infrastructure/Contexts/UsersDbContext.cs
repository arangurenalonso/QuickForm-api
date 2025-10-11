using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Base;
using QuickForm.Common.Domain.Method;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Application;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public sealed class UsersDbContext(
        DbContextOptions<UsersDbContext> options,
        IServiceProvider serviceProvider
    ) : DbContext(options), IUnitOfWork
{
    public required DbSet<RoleDomain> Role { get; set; }
    public required DbSet<AuthActionDomain> AuthAction { get; set; }
    public required DbSet<AuthActionTokenDomain> AuthActionToken { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Auth);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        base.OnModelCreating(modelBuilder);
    }
    public async Task<ResultT<int>> SaveChangesWithResultAsync(string classOrigin, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ITrackableEntity entity)
                {
                    entity.ClassOrigin = classOrigin;
                }
            }
            var result = await base.SaveChangesAsync(cancellationToken);
            return ResultT<int>.Success(result);
        }
        catch (Exception e)
        {
            var listResultError= CommonMethods.ConvertExceptionToResult(e, "Database Transaction");
            return ResultT<int>.FailureT(ResultType.DataBaseTransaction,listResultError);
        }
    }
    public IGenericUserRepository<TEntity, TEntityId> Repository<TEntity, TEntityId>()
     where TEntity : BaseDomainEntity<TEntityId>
     where TEntityId : EntityId
    {
        return serviceProvider.GetRequiredService<IGenericUserRepository<TEntity, TEntityId>>();
    }


}
