namespace QuickForm.Common.Domain;
public class StatusPropertiesDto : MasterUpdateBase
{
    public string Color { get; }
    public string? Icon { get; }

    public StatusPropertiesDto(
        string keyName,
        string? description,
        string color,
        string? icon,
        int? sortOrder = null
    ) : base(keyName, description, sortOrder)
    {
        Color = color;
        Icon = icon;
    }
}
