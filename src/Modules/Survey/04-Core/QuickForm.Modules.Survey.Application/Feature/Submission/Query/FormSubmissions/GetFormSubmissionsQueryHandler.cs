using System.Globalization;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class GetFormSubmissionsQueryHandler(
    IFormRepository _formRepository,
    IFormQueries _formQueries
) : IQueryHandler<GetFormSubmissionsQuery, FormSubmissionsResult>
{
    public async Task<ResultT<FormSubmissionsResult>> Handle(
        GetFormSubmissionsQuery request,
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

        var columns = await _formQueries.GetFormColumnsByIdFormAsync(
            request.FormId,
            cancellationToken
        );

        columns = AddSystemColumns(columns);

        var pagedRows = await _formQueries.GetFormRowsByIdFormAsync(
            request.FormId,
            skip,
            pageSize,
            cancellationToken
        );

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

        var resultRows = new PaginationResult<Dictionary<string, object?>>
        {
            Items = flatRows,
            TotalCount = pagedRows.TotalCount,
            PageSize = pagedRows.PageSize,
            CurrentPage = pagedRows.CurrentPage,
            TotalPages = pagedRows.TotalPages,
        };

        var result = new FormSubmissionsResult(columns, resultRows);
        return result;
    }

    private List<ColumnDto> AddSystemColumns(List<ColumnDto> columns)
    {
        var columnId = new ColumnDto
        {
            Key = "submissionId",
            Label = "Submission Id",
            Order = 0,
            Type = QuestionTypeType.InputTypeText.GetName(),
            IsKey = true,
            ShowInTable = false
        };

        var columnDate = new ColumnDto
        {
            Key = "submittedAt",
            Label = "Submitted At",
            Order = 1,
            Type = QuestionTypeType.InputTypeDatetime.GetName(),
            Pinned = "right"
        };

        columns.Add(columnId);
        columns.Add(columnDate);

        return columns
            .OrderBy(c => c.Order)
            .ToList();
    }
}
