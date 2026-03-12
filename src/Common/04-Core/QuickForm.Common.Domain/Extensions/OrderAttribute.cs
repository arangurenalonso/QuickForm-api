namespace QuickForm.Common.Domain;
[AttributeUsage(AttributeTargets.Field)]
public sealed class OrderAttribute : Attribute
{
    public int Description { get; }

    public OrderAttribute(int description)
    {
        Description = description;
    }
}

