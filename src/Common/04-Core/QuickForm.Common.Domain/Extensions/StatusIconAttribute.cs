namespace QuickForm.Common.Domain;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class StatusIconAttribute : Attribute
{
    public string Value { get; }

    public StatusIconAttribute(string value)
    {
        Value = value;
    }
}
