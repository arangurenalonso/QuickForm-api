using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record RegisterLeadCommand(
        string Name,
        string Email,
        string PhoneNumber,
        string Message
    )
    : ICommand<ResultTResponse<Guid>>;
