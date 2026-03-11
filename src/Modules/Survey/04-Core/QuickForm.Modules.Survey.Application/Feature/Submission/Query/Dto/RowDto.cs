
namespace QuickForm.Modules.Survey.Application;
public sealed class RowDto
{
    public Guid Id { get; init; }
    public DateTime SubmittedAt { get; init; }

    public Dictionary<string, object?> Cells { get; init; } = new();
}
