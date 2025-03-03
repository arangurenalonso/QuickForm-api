using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

public class LoginCommandHandler(
    IPasswordHashingService _passwordHashingService,
    ITokenService _tokenService,
    IUserRepository _userRepository
    ) : ICommandHandler<LoginCommand, string>
{

    public async Task<ResultT<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var resultUser = await ValidateInputData(request.Email);
        if (resultUser.IsFailure)
        {
            return ResultT<string>.Failure(resultUser.ResultType, resultUser.Errors);
        }
        var user = resultUser.Value;

        var resultPassword = ValidatePassword(user, request.Password);
        if (resultPassword.IsFailure)
        {
            return ResultT<string>.Failure(resultPassword.ResultType,resultPassword.Errors);
        }

        return CreateAuthenticationResult(user);
    }
    private async Task<ResultT<UserDomain>> ValidateInputData(string email)
    {
        var emailVO = EmailVO.Create(email);
        if (emailVO.IsFailure)
        {
            return ResultT<UserDomain>.Failure(ResultType.DomainValidation, emailVO.Errors);
        }
        var user = await _userRepository.GetByEmailAsync(emailVO.Value);
        if (user is null)
        {
            var error = ResultError.NullValue("Email", $"User with email '{email}' not found.");
            return ResultT<UserDomain>.Failure(ResultType.NotFound,error);
        }
        if (!user.IsEmailVerify)
        {
            var error = ResultError.InvalidOperation("Email", "Email has not been verified. Please verify your email to proceed.");
            return ResultT<UserDomain>.Failure(ResultType.DomainValidation, error);

        }
        return user;
    }

    private Result ValidatePassword(UserDomain user, string password)
    {
        var result = _passwordHashingService.VerifyPassword(password, user.PasswordHash.Value);

        if (!result)
        {
            var error = ResultError.InvalidOperation("Password","Invalid password");
            return Result.Failure(ResultType.MismatchValidation,error);
        }
        return Result.Success();

    }
    private ResultT<string> CreateAuthenticationResult(UserDomain user)
    {
        var resultTokenGenerate= _tokenService.GenerateToken(user.Id.Value,user.Name.Value,user.LastName?.Value,user.Email.Value);
        if (resultTokenGenerate.IsFailure)
        {
            return ResultT<string>.Failure(resultTokenGenerate.ResultType, resultTokenGenerate.Errors);
        }
        return resultTokenGenerate.Value;
    }
}
