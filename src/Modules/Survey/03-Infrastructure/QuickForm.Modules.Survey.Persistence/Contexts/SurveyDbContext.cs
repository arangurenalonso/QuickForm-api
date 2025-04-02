using System.Reflection;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain.Method;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain.Form;
using QuickForm.Common.Domain.Base;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public sealed class SurveyDbContext(DbContextOptions<SurveyDbContext> options) : DbContext(options), IUnitOfWork
{
    public required DbSet<Customer> Customers { get; set; }
    public required DbSet<FormDomain> Form { get; set; }

    public required DbSet<AuditLog> Audit { get; set; }

    public required DbSet<DataTypeDomain> DataType { get; set; }
    public required DbSet<AttributeDomain> Attribute { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Survey);

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
            Guid transactionId = Guid.NewGuid();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ITrackableEntity entity)
                {
                    entity.ClassOrigin = classOrigin;
                    entity.TransactionId = transactionId;
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
}
