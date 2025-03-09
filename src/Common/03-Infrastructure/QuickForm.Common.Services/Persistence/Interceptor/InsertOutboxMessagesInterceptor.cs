using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void InsertOutboxMessages(DbContext context)
    {

        // Get the tracked entities of type BaseDomainModel<AuditableEntity>
        List<BaseDomainEventEntity> trackedEntities = context.ChangeTracker
            .Entries<BaseDomainEventEntity>()
            .Select(entry => entry.Entity)
            .ToList();

        // Iterate through entities to get domain events
        var domainEventsList = trackedEntities
            .Select(entity =>
            {
                // Get domain events from the entity
                IReadOnlyCollection<IDomainEvent> domainEvents = entity.DomainEvents;

                // Clear domain events from the entity
                entity.ClearDomainEvents();

                return new
                {
                    Entity = entity,
                    DomainEvents = domainEvents
                };
            })
            .ToList(); // Convert to list for easier debugging

        // Merge all domain events into a single list
        var allDomainEvents = domainEventsList
           .SelectMany(e => e.DomainEvents.Select(domainEvent => new
           {
               DomainEvent = domainEvent,
               TrackingInfo = e.Entity.GetTrackingInfo() // Obtener ClassOrigin y TransactionId
           }))
           .ToList(); // Convert to list for easier debugging

        // Create outbox messages from domain events
        List<OutboxMessage> outboxMessages = allDomainEvents
              .Select(entry =>
              {
                  return new OutboxMessage
                  {
                      Id = entry.DomainEvent.Id,
                      Type = entry.DomainEvent.GetType().Name,
                      Content = JsonPrototype.Serialize(entry.DomainEvent),
                      OccurredOnUtc = entry.DomainEvent.OccurredOnUtc,
                      Status = OutboxStatus.NotProcessed,
                      TransactionId = entry.TrackingInfo.TransactionId, // Obtener TransactionId
                      ClassOrigin = entry.TrackingInfo.ClassOrigin // Obtener ClassOrigin
                  };
              })
             .ToList(); // Convert to list for easier debugging

        if (outboxMessages.Any())
        {
            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

    }
}
