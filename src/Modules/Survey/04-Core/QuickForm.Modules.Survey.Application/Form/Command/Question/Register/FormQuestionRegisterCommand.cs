using System.Text.Json;
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormQuestionRegisterCommand(
        Guid IdForm,
        List<QuestionDto> Questions
    )
    : ICommand<ResultResponse>;

public sealed record QuestionDto(
        Guid Id,
        string Type,
        JsonElement Properties
    );
