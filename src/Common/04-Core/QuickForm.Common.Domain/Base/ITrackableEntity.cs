namespace QuickForm.Common.Domain.Base;
public interface ITrackableEntity
{
    Guid EntityId { get;}
    string ClassOrigin { get; set; }
    Guid TransactionId { get; set; }
}
