using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;

public sealed record GetFormColumnsQuery()
    : IQuery<List<ColumnDto>>;
