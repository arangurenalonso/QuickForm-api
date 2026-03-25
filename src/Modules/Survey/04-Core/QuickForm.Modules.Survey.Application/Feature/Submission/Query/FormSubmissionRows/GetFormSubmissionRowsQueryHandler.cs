using System.Globalization;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class GetFormSubmissionRowsQueryHandler(
    IFormRepository _formRepository,
    ISubmissionQueries _submissionQueries
) : IQueryHandler<GetFormSubmissionRowsQuery, PaginationResult<Dictionary<string, object?>>>
{
    public async Task<ResultT<PaginationResult<Dictionary<string, object?>>>> Handle(
        GetFormSubmissionRowsQuery request,
        CancellationToken cancellationToken)
    {
        var form = await _formRepository.GetFormToCheckActionAsync(
            request.FormId,
            cancellationToken
        );

        if (form is null)
        {
            var error = ResultError.NullValue(
                "FormId",
                $"Form with id '{request.FormId}' not found."
            );
            return error;
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var skip = (page - 1) * pageSize;

        var pagedRowsResult = await _submissionQueries.SearchSubmissionsByIdFormAsync(
            request.FormId,
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

        var flatRows = new List<Dictionary<string, object?>>();

        foreach (var row in pagedRows.Items)
        {
            Dictionary<string, object?> rowDictionary = [];

            rowDictionary["submissionId"] = row.Id.ToString();
            rowDictionary["submittedAt"] = row.SubmittedAt.ToString(
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture
            );

            foreach (var cell in row.Cells)
            {
                rowDictionary[cell.Key] = cell.Value;
            }

            flatRows.Add(rowDictionary);
        }

        var result = new PaginationResult<Dictionary<string, object?>>
        {
            Items = flatRows,
            TotalCount = pagedRows.TotalCount,
            PageSize = pagedRows.PageSize,
            CurrentPage = pagedRows.CurrentPage,
            TotalPages = pagedRows.TotalPages,
        };

        return result;
    }
}
