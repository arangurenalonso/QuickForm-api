using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormPauseCommand(
        Guid Id
    )
    : ICommand<ResultResponse>;

