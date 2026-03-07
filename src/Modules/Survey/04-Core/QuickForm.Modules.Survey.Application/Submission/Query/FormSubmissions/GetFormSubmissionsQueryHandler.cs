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
        var form = await _formRepository.GetFormToCheckActionAsync(request.FormId, cancellationToken);
        if (form is null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.FormId}' not found.");
            return error;
        }

        var columns = await _formQueries.GetFormColumnsByIdFormAsync(request.FormId, cancellationToken);
        columns = AddSystemColumns(columns);
        var rows = await _formQueries.GetFormRowsByIdFormAsync(request.FormId,0,50, cancellationToken);

        var flatRows = new List<Dictionary<string, object?>>();
        foreach (var row in rows)
        {
            Dictionary<string, object?> rowDictionary = [];
            rowDictionary["submissionId"] = $"q_{row.Id}";
            rowDictionary["submittedAt"] = row.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            foreach (var cell in row.Cells)
            {
                rowDictionary[cell.Key] = cell.Value;
            }
            flatRows.Add(rowDictionary);
        }



        var result = new FormSubmissionsResult(columns, flatRows);
        return result;
    }
    private List<ColumnDto> AddSystemColumns(List<ColumnDto> columns)
    {
        var columnId = new ColumnDto()
        {
            Key = "submissionId",
            Label = "Submission Id",
            Order = 0,
            Type = QuestionTypeType.InputTypeText.GetName(),
            IsKey = true,
            ShowInTable = false
        };
        var columnDate = new ColumnDto()
        {
            Key = "submittedAt",
            Label = "Submitted At",
            Order = 1,
            Type = QuestionTypeType.InputTypeDatetime.GetName(),
            Pinned = "right",
        };


        columns.Add(columnId);
        columns.Add(columnDate);
        return columns
                .OrderBy(c => c.Order)
                .ToList();
    }

}
