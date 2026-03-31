using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormPauseCommand(
        Guid IdForm
    )
    : ICommand<ResultTResponse<FormViewModel>>;

