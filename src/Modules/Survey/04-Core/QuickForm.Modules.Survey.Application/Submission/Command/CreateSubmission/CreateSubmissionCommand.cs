using System.Text.Json;
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record CreateSubmissionCommand(
        Guid IdForm,
        Dictionary<string, JsonElement> request
    )
    : ICommand<ResultResponse>;
