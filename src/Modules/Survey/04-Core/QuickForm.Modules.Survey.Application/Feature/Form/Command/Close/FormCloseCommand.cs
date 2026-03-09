using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormCloseCommand(
        Guid Id
    )
    : ICommand<ResultResponse>;
