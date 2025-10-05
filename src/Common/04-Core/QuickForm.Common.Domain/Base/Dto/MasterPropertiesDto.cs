namespace QuickForm.Common.Domain;
public class MasterUpdateBase
{
    public string KeyName { get; init; }
    public string? Description { get; init; }
    public int? SortOrder { get; init; }

    public MasterUpdateBase() { }
    public MasterUpdateBase(string keyName, string? description = null, int? sortOrder=null)
    {
        KeyName = keyName;
        Description = description;
        SortOrder = sortOrder;
    }

}
