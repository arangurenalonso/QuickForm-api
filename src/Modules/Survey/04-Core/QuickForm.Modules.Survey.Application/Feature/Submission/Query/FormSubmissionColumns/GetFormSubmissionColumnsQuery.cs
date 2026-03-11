using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;

public sealed record GetFormSubmissionColumnsQuery(Guid FormId)
    : IQuery<List<ColumnDto>>;
