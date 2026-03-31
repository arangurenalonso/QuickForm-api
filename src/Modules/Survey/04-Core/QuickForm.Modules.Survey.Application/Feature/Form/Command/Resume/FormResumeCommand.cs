using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormResumeCommand(
        Guid IdForm
    )
   : ICommand<ResultTResponse<FormViewModel>>;


