namespace QuickForm.Common.Domain;

public abstract class BaseDomainEntity<TEntityId> : BaseAuditableEntity where TEntityId : class
{
    public TEntityId Id { get; init; }
    protected BaseDomainEntity(TEntityId id)
    {
        Id = id;
    }

    protected BaseDomainEntity() { }



}
