namespace QuickForm.Common.Domain.Base;
public interface IBaseDomainEntity
{
    Guid EntityId { get; }
    Dictionary<string, object?> GetProperties();
}
