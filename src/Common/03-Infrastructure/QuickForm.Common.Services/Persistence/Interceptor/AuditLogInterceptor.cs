using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Base;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace QuickForm.Common.Infrastructure.Persistence;

public class AuditLogInterceptor(
        IDateTimeProvider _dateTimeService
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
        var transactionId = Guid.NewGuid();

        var userConnected = "System";
       
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is IBaseDomainEntity)
            {
                var entityBase = entry.Entity as dynamic;
                var idEntity = entityBase.EntityId;
                var originClass = entityBase.OriginClass;
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
                            transactionId,
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
                            transactionId,
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
                            transactionId,
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
