using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

public class ResendEmailConfirmationCommandHandler(
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    IUserRepository userRepository,
    IAuthActionTokenHashingService _authActionTokenHashingService,
    IAuthActionEmailService _authActionEmailService
) : ICommandHandler<ResendEmailConfirmationCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var resultUser = await GetUser(request.Email);
        if (resultUser.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultUser.ResultType, resultUser.Errors);
        }

        var user = resultUser.Value;

        var idAuthAction = new MasterId(AuthActionType.EmailConfirmation.GetId());

        var addActionResult = user.AddAction(
            idAuthAction,
            _dateTimeProvider.UtcNow,
            _authActionTokenHashingService);

        if (addActionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(addActionResult.ResultType, addActionResult.Errors);
        }

        var createdToken = addActionResult.Value;
        var plainToken = createdToken.PlainTextToken;

        if (string.IsNullOrWhiteSpace(plainToken))
        {
            var error = ResultError.InvalidOperation("Token", "Plain text token was not generated.");
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, error);
        }

        _unitOfWork.Repository<UserDomain, UserId>().UpdateEntity(user);

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }

        await _authActionEmailService.SendEmailConfirmationAsync(user.Email.Value, plainToken, cancellationToken);
        createdToken.ClearPlainTextToken();

        return ResultResponse.Success("We have resent you an email with the confirmation token, please verify your email.");
    }

    private async Task<ResultT<UserDomain>> GetUser(string email)
    {
        var emailVO = EmailVO.Create(email);
        if (emailVO.IsFailure)
        {
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, emailVO.Errors);
        }

        var user = await userRepository.GetByEmailWithActiveAuthActionsLoadedAsync(emailVO.Value, _dateTimeProvider.UtcNow);
        if (user is null)
        {
            var error = ResultError.InvalidInput("Email", $"User with email '{email}' not found");
            return ResultT<UserDomain>.FailureT(ResultType.NotFound, error);
        }

        return user;
    }
}
