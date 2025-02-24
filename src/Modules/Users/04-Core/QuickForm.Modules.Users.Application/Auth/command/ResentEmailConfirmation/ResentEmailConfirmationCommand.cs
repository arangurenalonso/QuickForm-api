using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record ResentEmailConfirmationCommand(
    string Email
    ) : ICommand<ResultResponse>;
