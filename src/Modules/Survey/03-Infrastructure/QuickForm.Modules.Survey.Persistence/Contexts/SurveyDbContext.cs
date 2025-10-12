using System.Reflection;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain.Method;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Survey.Application;
using QuickForm.Common.Domain.Base;
using QuickForm.Modules.Survey.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace QuickForm.Modules.Survey.Persistence;
public sealed class SurveyDbContext(
        DbContextOptions<SurveyDbContext> options,
        IServiceProvider serviceProvider
    ) : DbContext(options), IUnitOfWork
{
    public required DbSet<QuestionDomain> Question { get; set; }
    public required DbSet<QuestionTypeAttributeDomain> QuestionTypeAttribute { get; set; }

    public required DbSet<QuestionTypeDomain> QuestionType { get; set; }
    public required DbSet<AttributeDomain> Attribute { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Survey);

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
            var listResultError = CommonMethods.ConvertExceptionToResult(e, "Database Transaction");
            return ResultT<int>.FailureT(ResultType.DataBaseTransaction, listResultError);
        }
    }

    public ISurveyRepository<TEntity, TEntityId> Repository<TEntity,TEntityId>()
     where TEntity : BaseDomainEntity<TEntityId>
     where TEntityId : EntityId
    {
        return serviceProvider.GetRequiredService<ISurveyRepository<TEntity, TEntityId>>();
    }


    public ISurveryMasterRepository<TEntity> Repository<TEntity>()
     where TEntity : BaseMasterEntity
    {
        return serviceProvider.GetRequiredService<ISurveryMasterRepository<TEntity>>();
    }

}
