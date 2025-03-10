namespace QuickForm.Common.Application;
public abstract class IntegrationEvent : IIntegrationEvent
{
    protected IntegrationEvent(Guid id, DateTime occurredOnUtc, Guid idOutboxMessage)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        IdOutboxMessage = idOutboxMessage;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }
    public Guid IdOutboxMessage { get; init; }
}
