using System.Text;
using QuickForm.Common.Application;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Service;

public sealed class AuthActionEmailService(
    IAzureBlobStorageService _blobStorageService,
    IAzureCommunicationEmailService _azureCommunicationEmailService,
    IDateTimeProvider _dateTimeProvider,
    ICommonOptionsProvider _commonOptionsProvider
) : IAuthActionEmailService
{
    public async Task SendEmailConfirmationAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var fileResult = await _blobStorageService.GetFileBlobAsync("template", "confirm_email.html", false);
        if (fileResult.IsFailure)
        {
            throw new Exception("The email confirmation template 'confirm_email.html' was not found in Blob Storage.");
        }

        var (fileStream, _) = fileResult.Value;
        await using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        var htmlTemplate = Encoding.UTF8.GetString(memoryStream.ToArray());

        string username = email.Split('@')[0];
        var year = _dateTimeProvider.UtcNow.Year;
        var baseUrl = _commonOptionsProvider.GetFrontEndApplicationUrl().ToString().TrimEnd('/');
        var verifyUrl = $"{baseUrl}/auth/email-confirmation?verification-code={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        var personalizedHtml = htmlTemplate
            .Replace("{name}", username)
            .Replace("{otp_code}", token)
            .Replace("{support_email}", "aranguren.alonso@gmail.com")
            .Replace("{year}", $"{year}")
            .Replace("{verify_url}", verifyUrl);

        await _azureCommunicationEmailService.SendEmailAsync(email, "Email Confirmation", personalizedHtml);
    }

    public async Task SendPasswordResetAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var fileResult = await _blobStorageService.GetFileBlobAsync("template", "reset_password.html", false);
        if (fileResult.IsFailure)
        {
            throw new Exception("The reset password template 'reset_password.html' was not found in Blob Storage.");
        }

        var (fileStream, _) = fileResult.Value;
        await using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        var htmlTemplate = Encoding.UTF8.GetString(memoryStream.ToArray());

        string username = email.Split('@')[0];
        var year = _dateTimeProvider.UtcNow.Year;
        var baseUrl = _commonOptionsProvider.GetFrontEndApplicationUrl().ToString().TrimEnd('/');
        var resetUrl = $"{baseUrl}/auth/reset-password?verification-code={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        var personalizedHtml = htmlTemplate
            .Replace("{name}", username)
            .Replace("{otp_code}", token)
            .Replace("{support_email}", "aranguren.alonso@gmail.com")
            .Replace("{year}", $"{year}")
            .Replace("{reset_url}", resetUrl);

        await _azureCommunicationEmailService.SendEmailAsync(email, "Password Reset", personalizedHtml);
    }
}
