namespace QuickForm.Modules.Survey.Application;

public sealed record SubmissionExportRow(
    Guid SubmissionId,
    DateTime SubmittedAtUtc,
    IReadOnlyList<SubmissionValueExport> Values
);

public sealed record SubmissionValueExport(
    Guid QuestionId,
    string RawJsonValue
);
