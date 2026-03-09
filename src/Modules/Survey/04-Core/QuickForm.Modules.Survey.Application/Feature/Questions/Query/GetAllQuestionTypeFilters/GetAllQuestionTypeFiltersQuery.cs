using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;

public sealed record GetAllQuestionTypeFiltersQuery()
    : IQuery<IReadOnlyList<QuestionTypeFiltersGroupDto>>;

public sealed record QuestionTypeFiltersGroupDto(
    Guid QuestionTypeId,
    string QuestionTypeKey,
    string QuestionTypeLabel,
    IReadOnlyList<QuestionTypeFilterOptionDto> Operators
);

public sealed record QuestionTypeFilterOptionDto(
    Guid Id,
    string Key,
    string Label,
    string UiControlType,
    string UiControlLabel
);
