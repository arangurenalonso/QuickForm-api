namespace QuickForm.Common.Application;
public abstract class IntegrationEvent : IIntegrationEvent
{
    protected IntegrationEvent(Guid id, DateTime occurredOnUtc, Guid idDomainEvent)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        IdDomainEvent = idDomainEvent;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }
    public Guid IdDomainEvent { get; init; }
}
