using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure.Persistence;

namespace QuickForm.Common.Infrastructure;
public class AuditFieldsInterceptor(
        IDateTimeProvider _dateTimeService,
        ICurrentUserService _currentUserService
    ) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
      DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditing(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditing(eventData.Context);
        }
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public void ApplyAuditing(DbContext context)
    {

        context.ChangeTracker.DetectChanges();

        // 0) Si cambió un owned/value object, fuerza al owner a Modified
        foreach (var owned in context.ChangeTracker.Entries()
                     .Where(e => e.Metadata.IsOwned() &&
                                 (e.State == EntityState.Added ||
                                  e.State == EntityState.Modified ||
                                  e.State == EntityState.Deleted)))
        {
            var owner = AuditHelper.ResolveOwner(owned);
            if (owner is not null && owner.State == EntityState.Unchanged)
            {
                owner.State = EntityState.Modified;
            }
        }

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
                    entry.Entity.MarkCreated(userConnected, now);
                    break;
                case EntityState.Modified:
                    if (entry.Entity.IsDeleted && entry.Entity.DeletedAt == null && entry.Entity.DeletedBy == null)
                    {
                        entry.Entity.MarkDeleted(userConnected, now);
                    }
                    else
                    {
                        entry.Entity.MarkUpdated(userConnected, now);
                    }
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.MarkDeleted(userConnected, now);
                    break;
                default:
                    break;
            }
        }
    }

}
