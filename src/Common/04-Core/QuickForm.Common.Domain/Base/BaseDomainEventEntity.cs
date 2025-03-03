using System.ComponentModel.DataAnnotations.Schema;

namespace QuickForm.Common.Domain;
public abstract class BaseDomainEventEntity
{
    [NotMapped]
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    protected void RaiseDomainEvents(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

}


