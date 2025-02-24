using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed class EmailVO
{
    public string Value { get; }

    private EmailVO(string value)
    {
        Value = value;
    }

    public static ResultT<EmailVO> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ResultError.EmptyValue("Email", "The email field cannot be empty. Please provide a value.");
        }

        if (!IsValidEmail(email))
        {
            return ResultError.InvalidFormat("Email", "The email format is invalid. Ensure it follows a valid format (e.g., name@domain.com).");
        }


        return new EmailVO(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static implicit operator string(EmailVO email) => email.Value;
}
