using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public class AuditableEntitySaveChangesInterceptor(
        IDateTimeProvider _dateTimeService
    ) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateEntities(eventData.Context);
        }
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext context)
    {

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            var userConnected = "System";

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreationAudit(userConnected, _dateTimeService.UtcNow);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModificationAudit(userConnected, _dateTimeService.UtcNow);
                    break;

                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    break;
            }
        }
    }

}
