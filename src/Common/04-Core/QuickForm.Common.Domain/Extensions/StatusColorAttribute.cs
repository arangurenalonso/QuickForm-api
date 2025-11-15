namespace QuickForm.Common.Domain;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class StatusColorAttribute : Attribute
{
    public string Value { get; }

    public StatusColorAttribute(string value)
    {
        Value = value;
    }
}
