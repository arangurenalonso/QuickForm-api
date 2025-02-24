using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record EmailConfirmationCommand(
    string Token
    ) : ICommand<ResultResponse>;
