using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormPublishCommand(
        Guid Id
    )
    : ICommand<ResultResponse>;
