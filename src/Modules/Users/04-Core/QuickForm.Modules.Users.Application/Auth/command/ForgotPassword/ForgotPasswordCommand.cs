using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record ForgotPasswordCommand(
    string Email
    ) : ICommand<ResultResponse>;
