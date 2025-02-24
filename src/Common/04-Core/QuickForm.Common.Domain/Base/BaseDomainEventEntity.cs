namespace QuickForm.Common.Domain;
public abstract class BaseDomainEventEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    protected void RaiseDomainEvents(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

}


