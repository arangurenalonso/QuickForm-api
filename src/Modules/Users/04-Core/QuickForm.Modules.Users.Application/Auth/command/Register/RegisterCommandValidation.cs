using FluentValidation;

namespace QuickForm.Modules.Users.Application;
public class RegisterCommandValidation : AbstractValidator<RegisterCommand>
{

    public RegisterCommandValidation()
    {
        RuleFor(c => c.Email).EmailAddress();
        RuleFor(c => c.Password).MinimumLength(6);
        RuleFor(c => c.ConfirmPassword).Equal(c => c.Password);
    }
}
