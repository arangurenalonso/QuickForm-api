using System.ComponentModel.DataAnnotations.Schema;
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

    [NotMapped]
    public Guid EntityId =>  Id.Value;

    [NotMapped]

    private string _originClass = string.Empty;

    [NotMapped]
    public string OriginClass
    {
        get => _originClass;
        set => _originClass = value;
    }   

}

