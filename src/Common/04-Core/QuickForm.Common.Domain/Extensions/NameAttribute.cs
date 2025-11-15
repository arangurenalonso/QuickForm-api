namespace QuickForm.Common.Domain;
[AttributeUsage(AttributeTargets.Field)]
public sealed class NameAttribute : Attribute
{
    public string Description { get; }

    public NameAttribute(string description)
    {
        Description = description;
    }
}
