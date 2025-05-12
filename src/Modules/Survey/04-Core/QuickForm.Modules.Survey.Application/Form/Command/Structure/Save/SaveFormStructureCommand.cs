using System.Text.Json;
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record SaveFormStructureCommand(
        Guid IdForm,
        List<SectionDto> Sections
    )
    : ICommand<ResultResponse>;

public sealed record SectionDto(
        Guid Id,
        string Title,
        string Description,
        List<QuestionDto> Questions
    );

public sealed record QuestionDto(
        Guid Id,
        string Type,
        JsonElement Properties
    );

public class QuestionSectionDto
{
    public Guid IdSection { get; set; }
    public QuestionDto Question { get; set; }
    public int Order { get; set; }
}
