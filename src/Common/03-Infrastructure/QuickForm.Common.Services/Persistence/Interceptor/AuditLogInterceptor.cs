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
        if (entry.Entity is AuditLog)
        {
            return false;
        }

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

                var (action, changes) = AuditHelper.CollectProperty(entry);
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
                    EntityState.Deleted => AuditHelper.GetFlattenedSnapshot(entry, AuditHelper.SnapshotKind.Original), // [CHANGED]
                    EntityState.Modified => AuditHelper.GetFlattenedSnapshot(entry, AuditHelper.SnapshotKind.Original), // [CHANGED]
                    _ => null
                };

                var currentDict = entry.State switch
                {
                    EntityState.Added => AuditHelper.GetFlattenedSnapshot(entry, AuditHelper.SnapshotKind.Current),    // [CHANGED]
                    EntityState.Deleted => null,
                    EntityState.Modified => AuditHelper.GetFlattenedSnapshot(entry, AuditHelper.SnapshotKind.Current), // [CHANGED]
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


}
public sealed class AuditHelper
{
    public enum SnapshotKind { Original, Current }
    public static Dictionary<string, object?> GetFlattenedSnapshot(EntityEntry ownerEntry, SnapshotKind kind)
    {
        var dict = new Dictionary<string, object?>();

        // 1) Owner
        var ownerValues = kind == SnapshotKind.Original
            ? ownerEntry.OriginalValues
            : ownerEntry.CurrentValues;

        foreach (var p in ownerValues.Properties)
        {
            if (p.IsPrimaryKey())
            {
                continue;
            }
            dict[p.Name] = ownerValues[p];
        }

        // 2) Owned (OwnsOne)
        foreach (var reference in ownerEntry.References
                                            .Where(r => r.TargetEntry is not null &&
                                                        r.TargetEntry.Metadata.IsOwned()))
        {
            var ownedEntry = reference.TargetEntry!;
            var nav = reference.Metadata.Name;

            // Caso reemplazo: Owned nuevo Added + Owner Modified ⇒ buscamos el viejo (Deleted) para "Original"
            EntityEntry? oldOwned = null;
            if (kind == SnapshotKind.Original &&
                ownedEntry.State == EntityState.Added &&
                ownerEntry.State == EntityState.Modified)
            {
                oldOwned = TryFindReplacedOwnedOldEntry(ownerEntry.Context, ownerEntry, ownedEntry);
            }

            // Elegimos la fuente según el snapshot
            PropertyValues sourceValues =
                kind == SnapshotKind.Original
                    ? (oldOwned?.CurrentValues ?? ownedEntry.OriginalValues)
                    : ownedEntry.CurrentValues;

            // En Current, si el owned está Deleted, ya no existe
            if (kind == SnapshotKind.Current && ownedEntry.State == EntityState.Deleted)
            {
                continue;
            }

            foreach (var p in sourceValues.Properties)
            {
                if (p.IsPrimaryKey())
                {
                    continue;
                }
                dict[$"{nav}.{p.Name}"] = sourceValues[p];
            }
        }

        // 3) (Opcional) OwnsMany – si no usas colecciones owned, puedes quitar este bloque
        foreach (var collection in ownerEntry.Collections
                                             .Where(c => c.Metadata.TargetEntityType.IsOwned()))
        {
            if (collection.CurrentValue is not System.Collections.IEnumerable enumerable)
            {
                continue;
            }

            int idx = 0;
            foreach (var item in enumerable)
            {
                if (item is null)
                { idx++; continue; }

                var itemEntry = ownerEntry.Context.Entry(item);
                PropertyValues itemValues =
                    kind == SnapshotKind.Original
                        ? itemEntry.OriginalValues
                        : itemEntry.CurrentValues;

                // En Original, si el elemento es Added, no hay original útil ⇒ saltar
                if (kind == SnapshotKind.Original && itemEntry.State == EntityState.Added)
                {
                    idx++;
                    continue;
                }
                // En Current, si el elemento es Deleted, ya no existe ⇒ saltar
                if (kind == SnapshotKind.Current && itemEntry.State == EntityState.Deleted)
                {
                    idx++;
                    continue;
                }

                foreach (var p in itemValues.Properties)
                {
                    if (p.IsPrimaryKey())
                    {
                        continue;
                    }
                    dict[$"{collection.Metadata.Name}[{idx}].{p.Name}"] = itemValues[p];
                }

                idx++;
            }
        }

        return dict;
    }

    private static EntityEntry? TryFindReplacedOwnedOldEntry(DbContext context, EntityEntry ownerEntry, EntityEntry ownedEntry)
    {
        // Mismo tipo owned
        var ownedType = ownedEntry.Metadata;

        // Relación de ownership (FK desde owned → owner)
        var ownership = ownedType.FindOwnership();
        if (ownership is null)
        {
            return null;
        }

        // Obtén las claves del owner
        var ownerKeyProps = ownership.PrincipalKey.Properties;
        var ownerKeyValues = ownerKeyProps.Select(p => ownerEntry.Property(p.Name).CurrentValue).ToArray();

        // Busca en el ChangeTracker un owned del mismo tipo, en estado Deleted,
        // que apunte al mismo owner (comparando valores de FK) y misma navegación.
        foreach (var e in context.ChangeTracker.Entries().Where(e => e.Metadata == ownedType && e.State == EntityState.Deleted))
        {
            // La FK del owned hacia el owner:
            var fkProps = ownership.Properties;

            bool sameOwner = fkProps.Select(p => e.Property(p.Name).CurrentValue)
                                    .SequenceEqual(ownerKeyValues);

            // El nombre de la navegación del owned hacia el owner es irrelevante aquí
            // lo importante es que el tipo y el owner coincidan.
            if (sameOwner)
            {
                return e;
            }
        }
        return null;
    }
    public static (string action, Dictionary<string, (object? Old, object? New)> changes) CollectProperty(EntityEntry entry)
    {
        var dict = new Dictionary<string, (object?, object?)>();
        string action = entry.State switch
        {
            EntityState.Added => "Added",
            EntityState.Modified => "Modified",
            EntityState.Deleted => "Deleted",
            _ => "None"
        };

        // 1) Props del owner (lo que ya tienes)
        foreach (var prop in entry.Properties)
        {
            if (prop.Metadata.IsPrimaryKey() || prop.IsTemporary)
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

        // 2) NUEVO: props de owned (OwnsOne) referenciados por el owner
        foreach (var reference in entry.References
                                       .Where(r => r.TargetEntry is not null &&
                                                   r.TargetEntry.Metadata.IsOwned()))
        {
            var ownedEntry = reference.TargetEntry!;
            string nav = reference.Metadata.Name;
            EntityEntry? oldOwned = null;
            if (ownedEntry.State == EntityState.Added && entry.State == EntityState.Modified)
            {
                oldOwned = TryFindReplacedOwnedOldEntry(entry.Context, entry, ownedEntry);
            }

            foreach (var prop in ownedEntry.Properties)
            {
                if (prop.Metadata.IsPrimaryKey() || prop.IsTemporary)
                {
                    continue;
                }

                // Si hay oldOwned (Deleted), toma sus valores como BEFORE
                object? before = oldOwned is not null
                    ? oldOwned.Property(prop.Metadata.Name).CurrentValue  // en Deleted, CurrentValue refleja el valor "viejo"
                    : prop.OriginalValue;

                object? after = prop.CurrentValue;

                bool changed = ownedEntry.State switch
                {
                    EntityState.Added => !Equals(before, after),   // ahora sí comparamos contra el viejo si lo encontramos
                    EntityState.Deleted => true,
                    EntityState.Modified => !Equals(before, after),
                    _ => !Equals(before, after)
                };

                if (changed)
                {
                    dict[$"{nav}.{prop.Metadata.Name}"] = (before, after);
                }
            }

            // Si solo cambió el owned, sube acción
            if (action == "None" &&
                ownedEntry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                action = "Modified";
            }
        }

        // 3) (Opcional) soportar OwnsMany
        foreach (var collection in entry.Collections
                                         .Where(c => c.Metadata.TargetEntityType.IsOwned()))
        {
            // Cada elemento de la colección tiene su propio TargetEntry en c.CurrentValue (IEnumerable)
            // Aquí suele requerir lógica extra para identificar el elemento (clave) y registrar cambios.
            // Puedes iterar c.Entries si usas EF >= 8: collection.GetEntries()
            // o mapear clave compuesta/pseudo-id en el diff para cada elemento.
        }

        return (action, dict);
    }

    
}
