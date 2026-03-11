using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class GetFormSubmissionColumnsQueryHandler(
    IFormRepository _formRepository,
    IFormQueries _formQueries
) : IQueryHandler<GetFormSubmissionColumnsQuery, List<ColumnDto>>
{
    public async Task<ResultT<List<ColumnDto>>> Handle(
        GetFormSubmissionColumnsQuery request,
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

        var columns = await _formQueries.GetFormColumnsByIdFormAsync(
            request.FormId,
            cancellationToken
        );

        columns = AddSystemColumns(columns);

        return columns;
    }

    private static List<ColumnDto> AddSystemColumns(List<ColumnDto> columns)
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
