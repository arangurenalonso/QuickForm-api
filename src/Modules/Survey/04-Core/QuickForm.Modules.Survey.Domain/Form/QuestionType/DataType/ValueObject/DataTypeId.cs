using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record DataTypeId(Guid Value) : EntityId(Value)
{
    public static DataTypeId Create() => new DataTypeId(Guid.NewGuid());
}
