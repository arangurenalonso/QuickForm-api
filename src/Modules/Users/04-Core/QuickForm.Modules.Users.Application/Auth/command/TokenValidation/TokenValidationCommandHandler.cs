using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;
public class TokenValidationCommandHandler(
    ITokenService _tokenService
    ) : ICommandHandler<TokenValidationCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(TokenValidationCommand request, CancellationToken cancellationToken)
    {
        var resultTokenResult = _tokenService.ValidateToken(request.Token);

        if (!resultTokenResult.IsSuccess)
        {
            return ResultT<ResultResponse>.Failure(ResultType.MismatchValidation,resultTokenResult.Errors);
        }

        return await Task.FromResult(ResultResponse.Success("Validation Token successful."));
    }

}
