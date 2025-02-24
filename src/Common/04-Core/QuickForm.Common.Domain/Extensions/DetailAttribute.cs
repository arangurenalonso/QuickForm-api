namespace QuickForm.Common.Domain;
[AttributeUsage(AttributeTargets.Field)]
public sealed class DetailAttribute : Attribute
{
    public string Description { get; }

    public DetailAttribute(string description)
    {
        Description = description;
    }
}
