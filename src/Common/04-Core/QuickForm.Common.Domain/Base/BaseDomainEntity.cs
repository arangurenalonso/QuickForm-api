using System.ComponentModel.DataAnnotations.Schema;
using QuickForm.Common.Domain.Base;

namespace QuickForm.Common.Domain;

public abstract class BaseDomainEntity<TEntityId> : BaseAuditableEntity, ITrackableEntity
    where TEntityId : EntityId
{
    public TEntityId Id { get; protected set; }
    protected BaseDomainEntity(TEntityId id)
    {
        Id = id;
    }

    protected BaseDomainEntity() { }



    [NotMapped]
    public string ClassOrigin { get; set; }

    [NotMapped]
    public Guid EntityId => Id.Value;

    public override TrackingInfo GetTrackingInfo()
        => new TrackingInfo(ClassOrigin);
}

