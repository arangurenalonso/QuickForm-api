using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record UpdateBasicInfoCommand(
        Guid Id,
        string Name,
        string? Description
    )
    : ICommand<ResultResponse>;
