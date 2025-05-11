using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record ValidateQuestionDtoCommand(
        List<QuestionDto> Questions
    )
    : ICommand;

