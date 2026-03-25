using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;

public sealed record GetMyFormsPaginationQuery(
    List<FiltersForm>? Filters,
    int Page = 1,
    int PageSize = 10
) : IQuery<PaginationResult<FormViewModel>>;


