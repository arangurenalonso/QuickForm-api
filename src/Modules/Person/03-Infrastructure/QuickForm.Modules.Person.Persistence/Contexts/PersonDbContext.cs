using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Base;
using QuickForm.Common.Domain.Method;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Person.Domain;
using QuickForm.Modules.Person.Application;
namespace QuickForm.Modules.Person.Persistence;
public sealed class PersonDbContext(
        DbContextOptions<PersonDbContext> options,
        IServiceProvider serviceProvider
    ) : DbContext(options), IUnitOfWork
{
    public required DbSet<AuditLog> Audit { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Person);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
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
            var listResultError = CommonMethods.ConvertExceptionToResult(e, "Database Transaction");
            return ResultT<int>.FailureT(ResultType.DataBaseTransaction, listResultError);
        }
    }

    public IGenericPersonRepository<TEntity, TEntityId> Repository<TEntity, TEntityId>()
     where TEntity : BaseDomainEntity<TEntityId>
     where TEntityId : EntityId
    {
        return serviceProvider.GetRequiredService<IGenericPersonRepository<TEntity, TEntityId>>();
    }


}

