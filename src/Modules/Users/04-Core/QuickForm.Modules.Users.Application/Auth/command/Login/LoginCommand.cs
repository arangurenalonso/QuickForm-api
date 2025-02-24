using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;
public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<string>;
