namespace QuickForm.Modules.Users.Application;

public interface IAuthActionEmailService
{
    Task SendEmailConfirmationAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendPasswordResetAsync(string email, string token, CancellationToken cancellationToken = default);
}
