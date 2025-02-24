
namespace QuickForm.Common.Domain;
public abstract class BaseAuditableEntity: BaseDomainEventEntity
{
    public string CreatedBy { get; set; }
    public string? ModifiedBy { get; set; } 
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }

    public void SetCreationAudit(string createdBy, DateTime createdDate)
    {
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        IsActive = true;
    }

    public void SetModificationAudit(string? modifiedBy, DateTime? modifiedDate)
    {
        ModifiedBy = modifiedBy;
        ModifiedDate = modifiedDate;
    }

    public virtual void Deactivate()
    {
        IsActive = false;
    }

    public virtual void Activate()
    {
        IsActive = true;
    }
}
