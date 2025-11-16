namespace QuickForm.Common.Domain;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class IconAttribute : Attribute
{
    public string Value { get; }

    public IconAttribute(string value)
    {
        Value = value;
    }
}
