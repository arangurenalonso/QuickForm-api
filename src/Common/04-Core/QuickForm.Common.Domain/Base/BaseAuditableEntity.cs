
namespace QuickForm.Common.Domain;
public abstract class BaseAuditableEntity: BaseDomainEventEntity
{
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? ModifiedBy { get; set; } 
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedBy { get; protected set; }


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
