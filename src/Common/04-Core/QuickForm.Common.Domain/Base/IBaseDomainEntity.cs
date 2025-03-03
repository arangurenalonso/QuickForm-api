using System.ComponentModel.DataAnnotations.Schema;

namespace QuickForm.Common.Domain.Base;
public interface IBaseDomainEntity
{
    Guid EntityId { get; }
    string OriginClass { get; set; }
}
