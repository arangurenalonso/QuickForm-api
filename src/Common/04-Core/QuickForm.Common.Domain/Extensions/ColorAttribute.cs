namespace QuickForm.Common.Domain;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class ColorAttribute : Attribute
{
    public string Value { get; }

    public ColorAttribute(string value)
    {
        Value = value;
    }
}
