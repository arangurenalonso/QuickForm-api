using System.Text.Json;
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;

public sealed record GetFormSubmissionsQuery(
        Guid FormId
    )
    : IQuery<FormSubmissionsResult>;

public sealed record FormSubmissionsResult(
    List<ColumnDto> Columns,
    IReadOnlyList<Dictionary<string, object?>> Rows
);

public sealed class ColumnDto
{
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public int Order { get; set; }  
    public string Type { get; set; } = default!;
    public bool IsKey { get; set; }
    public bool ShowInTable { get; set; } = true;
    public string? Pinned { get; set; }
}
public sealed class RowDto
{
    public Guid Id { get; init; }
    public DateTime SubmittedAt { get; init; }

    public Dictionary<string, object?> Cells { get; init; } = new();
}
