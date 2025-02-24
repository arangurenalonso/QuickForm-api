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
        var trackedEntities = context.ChangeTracker
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

                return domainEvents;
            })
            .ToList(); // Convert to list for easier debugging

        // Merge all domain events into a single list
        var allDomainEvents = domainEventsList
            .SelectMany(domainEvents => domainEvents)
            .ToList(); // Convert to list for easier debugging

        // Create outbox messages from domain events
        var outboxMessages = allDomainEvents
             .Select(domainEvent => new OutboxMessage
             {
                 Id = domainEvent.Id,
                 Type = domainEvent.GetType().Name,
                 Content = JsonPrototype.Serialize(domainEvent),
                 OccurredOnUtc = domainEvent.OccurredOnUtc,
                 Status = OutboxStatus.NotProcessed
             })
            .ToList(); // Convert to list for easier debugging

        if (outboxMessages.Any())
        {
            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

    }
}
