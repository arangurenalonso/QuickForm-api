namespace QuickForm.Common.Domain;
public abstract class MasterUpdateBase
{
    public required string KeyName { get; init; }
    public string? Description { get; init; }
    public int? SortOrder { get; init; }
}
