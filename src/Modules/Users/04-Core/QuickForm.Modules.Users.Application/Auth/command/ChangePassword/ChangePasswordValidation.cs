using FluentValidation;

namespace QuickForm.Modules.Users.Application;
public class ChangePasswordValidation : AbstractValidator<ChangePasswordCommand>
{

    public ChangePasswordValidation()
    {
        RuleFor(c => c.CurrentPassword).NotNull().NotEmpty();
        RuleFor(c => c.NewPassword).NotNull().NotEmpty();
    }
}
