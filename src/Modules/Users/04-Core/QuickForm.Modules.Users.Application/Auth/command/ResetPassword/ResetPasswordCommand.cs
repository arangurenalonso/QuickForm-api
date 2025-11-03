using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;
public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string Password,
    string ConfirmPassword
    ) : ICommand<ResultResponse>;
