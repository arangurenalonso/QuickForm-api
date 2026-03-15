using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record ResendEmailConfirmationCommand(
    string Email
    ) : ICommand<ResultResponse>;
