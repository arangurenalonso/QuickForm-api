using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;
public sealed record FiltersForm(
        string ColumnKey,
        Guid QuestionTypeId,
        string QuestionTypeKey,
        Guid OperatorId,
        string OperatorKey,
        JsonElement? Value = null,
        JsonElement? SecondValue = null
    );

