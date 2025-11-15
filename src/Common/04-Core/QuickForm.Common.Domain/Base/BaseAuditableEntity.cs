
namespace QuickForm.Common.Domain;
public abstract class BaseAuditableEntity: BaseDomainEventEntity
{
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? ModifiedBy { get; private set; } 
    public DateTime? ModifiedDate { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }


    public void MarkCreated(string createdBy, DateTime createdDate)
    {
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        IsDeleted = false;
    }

    public void MarkUpdated(string modifiedBy, DateTime modifiedDate)
    {
        ModifiedBy = modifiedBy;
        ModifiedDate = modifiedDate;
    }
    public void MarkDeleted(string deletedBy, DateTime deletedAt)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = deletedAt;
    }

}
