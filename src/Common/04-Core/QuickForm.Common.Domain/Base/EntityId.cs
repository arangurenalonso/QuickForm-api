namespace QuickForm.Common.Domain;

public abstract record EntityId(Guid Value)
{
    public override string ToString() => Value.ToString();
}
