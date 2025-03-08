using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;
public class RegisterCommandHandler(
        IUnitOfWork _unitOfWork,
        IUserRepository _userRepository,
        IPasswordHashingService _passwordHashingService,
        IDateTimeProvider _dateTimeProvider
    ) : ICommandHandler<RegisterCommand, ResultResponse>
{

    public async Task<ResultT<ResultResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var resultValidateInputData = await ValidateInputData(request.Email);
        if (resultValidateInputData.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultValidateInputData.ResultType,resultValidateInputData.Errors);
        }

        var newUserResult = CreateUser(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);
        if (newUserResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(newUserResult.ResultType,newUserResult.Errors);
        }

        var transactionResut = await _unitOfWork.SaveChangesWithResultAsync(nameof(RegisterCommandHandler),cancellationToken);

        if (transactionResut.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(transactionResut.ResultType,transactionResut.Errors);
        }
        return ResultResponse.Success("User created successfully.");
    }
    private async Task<Result> ValidateInputData(string email)
    {
        var emailVO = EmailVO.Create(email);
        if (emailVO.IsFailure)
        {
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, emailVO.Errors);
        }
        if (await _userRepository.IsEmailExistsAsync(emailVO.Value))
        {
            var error = ResultError.DuplicateValueAlreadyInUse("Email",  $"User with email '{email}' already exist");
            return Result.Failure(ResultType.Conflict,error);
        }
        return Result.Success();
    }

    private ResultT<UserDomain> CreateUser(
        string firstName,
        string? lastName,
        string email,
        string password)
    {
        var userDomainResult = UserDomain.Create(
            firstName,
            lastName,
            email,
            password,
            _passwordHashingService,
                _dateTimeProvider.UtcNow);

        if (userDomainResult.IsFailure)
        {
            var errorList = userDomainResult.Errors;
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation,errorList);
        }
        var userDomain = userDomainResult.Value;

        _userRepository.Insert(userDomain);
        return userDomain;
    }

}
