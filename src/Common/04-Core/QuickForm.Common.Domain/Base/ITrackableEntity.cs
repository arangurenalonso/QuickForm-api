namespace QuickForm.Common.Domain.Base;
public interface ITrackableEntity
{
    string ClassOrigin { get; set; }
    Guid TransactionId { get; set; }
}
