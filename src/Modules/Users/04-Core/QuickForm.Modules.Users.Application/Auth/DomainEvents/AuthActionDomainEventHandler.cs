using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using System.Text;

namespace QuickForm.Modules.Users.Application;

internal sealed class AuthActionDomainEventHandler(
    IUserRepository _userRepository,
    IAzureBlobStorageService _blobStorageService,
    IAzureCommunicationEmailService _azureCommunicationEmailService,
    IDateTimeProvider _dateTimeProvider,
    ICommonOptionsProvider _commonOptionsProvider
    ) : DomainEventHandler<AuthActionDomainEvent>
{

    public override async Task Handle(AuthActionDomainEvent notification, CancellationToken cancellationToken=default)
    {
        var user = await _userRepository.GetByIdWithActiveAuthActionsAsync(notification.UserId, _dateTimeProvider.UtcNow);
        if (user == null)
        {
            throw new Exception($"User with ID '{notification.UserId.Value}' not found. Please ensure the user exists in the system.");
        }

        var authActionToken = user.AuthActionTokens.FirstOrDefault(x => x.IdUserAction == notification.IdAuthAction);
        if (authActionToken == null)
        {
            throw new Exception($"No AuthActionToken was generated for the AuthAction with ID '{notification.IdAuthAction.Value}'. Ensure the action is correctly initiated.");
        }

        switch (notification.IdAuthAction.Value)
        {
            case var id when id == AuthActionType.EmailConfirmation.GetId():
                await SentConfirmationEmail(user, authActionToken.Token.Value);
                break;
            case var id when id == AuthActionType.RecoveryPassword.GetId():
                await SentResetPasswordEmail(user, authActionToken.Token.Value);
                break;
            default:
                throw new Exception($"Unknown authentication action with ID '{notification.IdAuthAction.Value}'. Please verify the action type.");
        }
    }

    private async Task SentConfirmationEmail(UserDomain user, string token)
    {

        var fileResult = await _blobStorageService.GetFileBlobAsync("template", "confirm_email.html", false);

        if (fileResult.IsFailure)
        {
            throw new Exception("The email confirmation template 'confirm_email.html' was not found in Blob Storage. Ensure that the template exists and is correctly configured.");
        }

        var (fileStream, _) = fileResult.Value;

        await using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        var htmlTemplate = Encoding.UTF8.GetString(fileBytes);
        string username = user.Email.Value.Split('@')[0];
        var personalizedHtml = htmlTemplate
            .Replace("{{name}}", username)
            .Replace("{{link_confirm}}", $"{_commonOptionsProvider.GetFrontEndApplicationUrl().ToString()}auth/email-confirmation?token={token}&email={user.Email.Value}");
        await _azureCommunicationEmailService.SendEmailAsync(user.Email.Value, "Email Confirmation", personalizedHtml);

    }

    private async Task SentResetPasswordEmail(UserDomain user, string token)
    {
        var fileResult = await _blobStorageService.GetFileBlobAsync("template", "reset_password.html", false);

        if (fileResult.IsFailure)
        {
            throw new Exception("The reset password template 'reset_password.html' was not found in Blob Storage. Ensure that the template exists and is correctly configured.");
        }

        var (fileStream, _) = fileResult.Value;
        await using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        var htmlTemplate = Encoding.UTF8.GetString(fileBytes);
        var personalizedHtml = htmlTemplate
            .Replace("{{link_reset}}", $"{_commonOptionsProvider.GetFrontEndApplicationUrl().ToString()}auth/reset-password?token={token}&email={user.Email.Value}");

        await _azureCommunicationEmailService.SendEmailAsync(user.Email.Value, "Password Reset", personalizedHtml);
    }
}
