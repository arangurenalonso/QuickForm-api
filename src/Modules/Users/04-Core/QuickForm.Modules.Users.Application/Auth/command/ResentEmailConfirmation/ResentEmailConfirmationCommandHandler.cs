using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;
public class ResentEmailConfirmationCommandHandler(
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    IUserRepository userRepository
    ) : ICommandHandler<ResentEmailConfirmationCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(ResentEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var resultUser = await GetUser(request.Email);
        if (resultUser.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultUser.ResultType, resultUser.Errors);
        }
        var user = resultUser.Value;

        var idAuthActionPasswordRecovery = AuthActionType.EmailConfirmation.GetId();
        var idAuthAction = new MasterId(idAuthActionPasswordRecovery);
        user.AddAction(idAuthAction, _dateTimeProvider.UtcNow);


        _unitOfWork.Repository<UserDomain, UserId>().UpdateEntity(user);

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }
        var msg = "We have resent you an email with the confirmation token, please verify your email.";
        return ResultResponse.Success(msg);
    }
    private async Task<ResultT<UserDomain>> GetUser(string email)
    {
        var emailVO = EmailVO.Create(email);
        if (emailVO.IsFailure)
        {
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, emailVO.Errors);
        }
        var user = await userRepository.GetByEmailWithActiveAuthActionsAsync(emailVO.Value, _dateTimeProvider.UtcNow);
        if (user is null)
        {
            var error = ResultError.InvalidInput("Email", $"User with email '{email}' not found");
            return ResultT<UserDomain>.FailureT(ResultType.NotFound, error);
        }
        return user;
    }



}
