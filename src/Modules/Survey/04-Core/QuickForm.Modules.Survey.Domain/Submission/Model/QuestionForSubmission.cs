namespace QuickForm.Modules.Survey.Domain;
public sealed record QuestionForSubmission(
    Guid QuestionId,
    Guid QuestionTypeId,
    IReadOnlyList<(Guid AttributeId, string? Value)> Attributes,
    IReadOnlyList<(Guid RuleId, string? Value, string? Message)> Rules
);
