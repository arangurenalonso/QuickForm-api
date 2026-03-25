using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class GetMyFormsPaginationQueryHandler(
    IFormQueries _formQueries,
    ICurrentUserService _currentUserService
) : IQueryHandler<GetMyFormsPaginationQuery, PaginationResult<FormViewModel>>
{
    public async Task<ResultT<PaginationResult<FormViewModel>>> Handle(
        GetMyFormsPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var userIdResult = _currentUserService.UserId;
        if (userIdResult.IsFailure)
        {
            return userIdResult.Errors;
        }
        Guid userId = userIdResult.Value;


        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var skip = (page - 1) * pageSize;

        var pagedRowsResult = await _formQueries.SearchFormAsync(
            userId,
            request.Filters,
            skip,
            pageSize,
            cancellationToken
        );

        if (pagedRowsResult.IsFailure)
        {
            return pagedRowsResult.Errors;
        }
        var pagedRows = pagedRowsResult.Value;


        return pagedRows;
    }
}
