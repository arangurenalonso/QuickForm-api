using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class GetFormColumnsQueryHandle(
    IFormQueries _formQueries
) : IQueryHandler<GetFormColumnsQuery, List<ColumnDto>>
{
    public async Task<ResultT<List<ColumnDto>>> Handle(
        GetFormColumnsQuery request,
        CancellationToken cancellationToken)
    {

        var listStatusOption = await _formQueries.GetFormStatusAsOptionsViewModel(
            cancellationToken
        );

        var columns = GetSystemColumns(listStatusOption);

        return columns;
    }

    private static List<ColumnDto> GetSystemColumns(List<OptionsViewModel> listStatusOption)
    {
        List<ColumnDto> columns = new List<ColumnDto>();
        var columnId = new ColumnDto
        {
            Key = "id",
            Label = "Form Id",
            Order = 0,
            QuestionTypeKey = QuestionTypeType.InputTypeText.GetName(),
            QuestionTypeId = QuestionTypeType.InputTypeText.GetId(),
            IsKey = true,
            ShowInTable = false
        };

        columns.Add(columnId);


        var columnName = new ColumnDto
        {
            Key = "name",
            Label = "Name",
            Order = 2,
            QuestionTypeKey = QuestionTypeType.InputTypeText.GetName(),
            QuestionTypeId = QuestionTypeType.InputTypeText.GetId(),
        };

        columns.Add(columnName);

        var columnDescription = new ColumnDto
        {
            Key = "description",
            Label = "Description",
            Order = 3,
            QuestionTypeKey = QuestionTypeType.InputTypeText.GetName(),
            QuestionTypeId = QuestionTypeType.InputTypeText.GetId(),
        };

        columns.Add(columnDescription);

        var columnUpdated = new ColumnDto
        {
            Key = "updated",
            Label = "Last Edit",
            Order = 4,
            QuestionTypeKey = QuestionTypeType.InputTypeDatetime.GetName(),
            QuestionTypeId = QuestionTypeType.InputTypeDatetime.GetId(),
        };
        columns.Add(columnUpdated);

        var columnSubmissions = new ColumnDto
        {
            Key = "submissions",
            Label = "Submissions",
            Order = 5,
            QuestionTypeKey = QuestionTypeType.InputTypeInteger.GetName(),
            QuestionTypeId = QuestionTypeType.InputTypeInteger.GetId()
        };
        columns.Add(columnSubmissions);

        var columStatus = new ColumnDto
        {
            Key = "status",
            Label = "Status",
            Order = 101,
            QuestionTypeKey = QuestionTypeType.InputTypeSelect.GetName(),
            QuestionTypeId = QuestionTypeType.InputTypeSelect.GetId(),
            Pinned = "right",
            options = listStatusOption
        };
        columns.Add(columStatus);

        var columnAction = new ColumnDto
        {
            Key = "action",
            Label = "Action",
            Order = 102,
            QuestionTypeKey = QuestionTypeType.Action.GetName(),
            QuestionTypeId = QuestionTypeType.Action.GetId(),
            Pinned = "right",
            ShowInFilter = false
        };
        columns.Add(columnAction);


        return columns
            .OrderBy(c => c.Order)
            .ToList();
    }
}
