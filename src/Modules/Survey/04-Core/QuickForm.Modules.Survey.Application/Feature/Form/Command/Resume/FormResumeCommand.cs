using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormResumeCommand(
        Guid Id
    )
    : ICommand<ResultResponse>;

