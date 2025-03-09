using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public class AuditFieldsInterceptor(
        IDateTimeProvider _dateTimeService,
        ICurrentUserService _currentUserService
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
        var userConnected = "System";
        string userFullName = _currentUserService.UserFullName;
        ResultT<Guid> resultUserId= _currentUserService.UserId;
        if (resultUserId.IsSuccess)
        {
            userConnected = $"{resultUserId.Value.ToString()}|{userFullName}";
        }
        var now = _dateTimeService.UtcNow;
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreationAudit(userConnected, now);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModificationAudit(userConnected, now);
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
