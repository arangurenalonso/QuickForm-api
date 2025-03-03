using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain.AuthActionToken.Enum;
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
            return ResultT<ResultResponse>.Failure(resultUser.ResultType, resultUser.Errors);
        }
        var user = resultUser.Value;

        var idAuthActionPasswordRecovery = AuthActionType.EmailConfirmation.GetId();
        var idAuthAction = new AuthActionId(idAuthActionPasswordRecovery);
        user.AddAction(idAuthAction, _dateTimeProvider.UtcNow);

        userRepository.Update(user);

        var result = await _unitOfWork.SaveChangesWithResultAsync(nameof(ResentEmailConfirmationCommandHandler),cancellationToken);
         
        if (result.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(result.ResultType, result.Errors);
        }

        return ResultResponse.Success("Email Confirmation sent successfully.");
    }
    private async Task<ResultT<UserDomain>> GetUser(string email)
    {
        var emailVO = EmailVO.Create(email);
        if (emailVO.IsFailure)
        {
            return ResultT<UserDomain>.Failure(ResultType.DomainValidation, emailVO.Errors);
        }
        var user = await userRepository.GetByEmailWithActiveAuthActionsAsync(emailVO.Value, _dateTimeProvider.UtcNow);
        if (user is null)
        {
            var error = ResultError.InvalidInput("Email", $"User with email '{email}' not found");
            return ResultT<UserDomain>.Failure(ResultType.NotFound, error);
        }
        return user;
    }



}
