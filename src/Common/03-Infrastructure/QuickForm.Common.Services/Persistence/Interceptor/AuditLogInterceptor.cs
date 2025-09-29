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

    private static bool ShouldAudit(EntityEntry entry)
    {
        // 1) Ignora owned types (se auditan como parte del owner)
        if (entry.Metadata.IsOwned())
        {
            return false;
        }

        // 2) Ignora la propia tabla de auditoría
        if (entry.Entity is AuditLog)
        {
            return false;
        }

        // 3) Sólo Added/Modified/Deleted
        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
    }
    public void OnAfterSaveChanges(DbContext? context)
    {
        if (context == null)
        {
            return;
        }
        var now = _dateTimeService.UtcNow;

        var userConnected = "System";
        string userFullName = _currentUserService.UserFullName;
        ResultT<Guid> resultUserId = _currentUserService.UserId;
        if (resultUserId.IsSuccess)
        {
            userConnected = $"{resultUserId.Value.ToString()}|{userFullName}";
        }
        var auditList = new List<AuditLog>();
        context.ChangeTracker.DetectChanges();

        Guid transactionId = Guid.NewGuid();
        foreach (var entry in context.ChangeTracker.Entries().Where(ShouldAudit))
        {

            if (entry.Entity is ITrackableEntity entity)
            {
                Guid idEntity = entity.EntityId;
                var tableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name;

                var (action, changes) = ChangesCollector.Collect(entry);
                if (action == "None" )
                {
                    continue;
                }
                if (action == "Modified" && changes.Count == 0)
                {
                    continue;
                }
                var originalDict = entry.State switch
                {
                    EntityState.Added => null,                                 
                    EntityState.Deleted => GetPropertiesToDictionary(entry.OriginalValues),
                    EntityState.Modified => GetPropertiesToDictionary(entry.OriginalValues),
                    _ => null
                };

                var currentDict = entry.State switch
                {
                    EntityState.Added => GetPropertiesToDictionary(entry.CurrentValues),
                    EntityState.Deleted => null,                                 
                    EntityState.Modified => GetPropertiesToDictionary(entry.CurrentValues),
                    _ => null
                };

                var originalJson = originalDict is null
                    ? null
                    : JsonPrototype.Serialize(originalDict, SerializerSettings.CleanInstance);

                var currentJson = currentDict is null
                    ? null
                    : JsonPrototype.Serialize(currentDict, SerializerSettings.CleanInstance);

                var changedValues = changes.ToDictionary(
                                            kv => kv.Key,
                                            kv => new { Before = kv.Value.Old, After = kv.Value.New }
                                        );

                var changesJson = JsonPrototype.Serialize(changedValues, SerializerSettings.CleanInstance);


                string originClass = entity.ClassOrigin??"";

                var opType = entry.State switch
                {
                    EntityState.Added => AuditOperacionType.Added,
                    EntityState.Modified => AuditOperacionType.Modified,
                    EntityState.Deleted => AuditOperacionType.Deleted,
                    _ => AuditOperacionType.Modified
                };
                var audit = AuditLog.Create(
                          idEntity,
                          now,
                          tableName,
                          opType,
                          entry.State.ToString(),
                          originalJson,
                          currentJson,
                          changesJson,
                          transactionId,
                          userConnected,
                          originClass
                      );

                auditList.Add(audit);
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
public sealed class ChangesCollector
{
    public static (string action, Dictionary<string, (object? Old, object? New)> changes)
        Collect(EntityEntry entry)
    {
        var dict = new Dictionary<string, (object?, object?)>();
        string action = entry.State switch
        {
            EntityState.Added => "Added",
            EntityState.Modified => "Modified",
            EntityState.Deleted => "Deleted",
            _ => "None"
        };

        foreach (var prop in entry.Properties)
        {
            if (prop.Metadata.IsPrimaryKey())
            {
                continue;
            }
            if (prop.IsTemporary)
            {
                continue;
            }

            object? original = prop.OriginalValue;
            object? current = prop.CurrentValue;

            bool changed = entry.State switch
            {
                EntityState.Added => true,
                EntityState.Deleted => true,
                EntityState.Modified => !Equals(original, current),
                _ => false
            };

            if (changed)
            {
                dict[prop.Metadata.Name] = (original, current);
            }
        }

        return (action, dict);
    }
}
