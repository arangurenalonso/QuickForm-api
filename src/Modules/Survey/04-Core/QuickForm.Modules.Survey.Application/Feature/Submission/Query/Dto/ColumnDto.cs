
namespace QuickForm.Modules.Survey.Application;
public sealed class ColumnDto
{

    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public int Order { get; set; }
    public Guid QuestionTypeId { get; set; } 
    public string QuestionTypeKey { get; set; } = default!;
    public bool IsKey { get; set; }
    public bool ShowInTable { get; set; } = true;
    public string? Pinned { get; set; }
}
