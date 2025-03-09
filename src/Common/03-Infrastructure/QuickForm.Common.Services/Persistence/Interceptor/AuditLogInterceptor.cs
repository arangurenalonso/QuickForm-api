using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Base;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MassTransit;

namespace QuickForm.Common.Infrastructure.Persistence;

public class AuditLogInterceptor(
        IDateTimeProvider _dateTimeService,
        ICurrentUserService _currentUserService
    ) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        OnAfterSaveChanges(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void OnAfterSaveChanges(DbContext? context)
    {
        if (context == null)
        {
            return;
        }
        var now = _dateTimeService.UtcNow;
        var auditList = new List<AuditLog>();

        var userConnected = "System";
        string userFullName = _currentUserService.UserFullName;
        ResultT<Guid> resultUserId = _currentUserService.UserId;
        if (resultUserId.IsSuccess)
        {
            userConnected = $"{resultUserId.Value.ToString()}|{userFullName}";
        }

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is BaseDomainEntity<EntityId> entity)
            {
                Guid idEntity = entity.Id.Value;
                string originClass = entity.ClassOrigin;
                switch (entry.State)
                {
                    case EntityState.Added:
                        var auditCreate = AuditLog.Create(
                            Guid.NewGuid(),
                            idEntity,
                            now,
                            entry.Metadata.GetTableName(),
                            AuditOperacionType.Added,
                            entry.State.ToString(),
                            null, 
                            JsonPrototype.Serialize(GetPropertiesToDictionary(entry.CurrentValues), SerializerSettings.CleanInstance),
                            null,
                            entity.TransactionId,
                            userConnected,
                            originClass
                        );
                        auditList.Add(auditCreate);
                        break;
                    case EntityState.Modified:

                        var changedValues = entry.Properties
                                            .Where(p => !Equals(p.OriginalValue, p.CurrentValue))
                                            .ToDictionary(
                                                p => p.Metadata.Name,
                                                p => new { Before = p.OriginalValue, After = p.CurrentValue }
                                            );
                        var auditModified = AuditLog.Create(
                            Guid.NewGuid(),
                            idEntity,
                            now,
                            entry.Metadata.GetTableName(),
                            AuditOperacionType.Modified,
                            entry.State.ToString(),
                            JsonPrototype.Serialize(GetPropertiesToDictionary(entry.OriginalValues), SerializerSettings.CleanInstance),
                            JsonPrototype.Serialize(GetPropertiesToDictionary(entry.CurrentValues), SerializerSettings.CleanInstance),
                            JsonPrototype.Serialize(changedValues, SerializerSettings.CleanInstance),
                            entity.TransactionId,
                            userConnected,
                            originClass
                        );
                        
                        auditList.Add(auditModified);

                        break;

                    case EntityState.Deleted:
                        var auditDeleted = AuditLog.Create(
                            Guid.NewGuid(),
                            idEntity,
                            now,
                            entry.Metadata.GetTableName(),
                            AuditOperacionType.Deleted,
                            entry.State.ToString(),
                            JsonPrototype.Serialize(GetPropertiesToDictionary(entry.OriginalValues), SerializerSettings.CleanInstance),
                            null,
                            null,
                            entity.TransactionId,
                            userConnected,
                            originClass
                        );
                        auditList.Add(auditDeleted);
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        break;

                }
            }
        }
        if (auditList.Any())
        {
            context.Set<AuditLog>().AddRange(auditList);
        }
    }
    private static Dictionary<string, object?> GetPropertiesToDictionary(PropertyValues values)
    {

        return values.EntityType.GetProperties().ToDictionary(p => p.Name, p => values[p]);
    }

}
