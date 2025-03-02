using System.Reflection;
using QuickForm.Common.Domain.Base;

namespace QuickForm.Common.Domain;

public abstract class BaseDomainEntity<TEntityId> : BaseAuditableEntity, IBaseDomainEntity
    where TEntityId : EntityId
{
    public TEntityId Id { get; init; }
    protected BaseDomainEntity(TEntityId id)
    {
        Id = id;
    }

    protected BaseDomainEntity() { }

    public Guid EntityId =>  Id.Value;
    public virtual Dictionary<string, object?> GetProperties()
    {
        return new Dictionary<string, object?> { { "Id", Id.Value } };
    }
}
