using MediatR;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application.Forms.Queries;

namespace QuickForm.Modules.Survey.Application;
public sealed class FormOwnershipBehavior<TRequest, TResponse>(
    ICurrentUserService currentUser,
    IFormQueries formQueries
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequireFormOwnership
{

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {

        var userIdResult = currentUser.UserId;
        if (userIdResult.IsFailure)
        {
            return ResultHelper.CreateFailureResponse<TResponse>(ResultType.Unauthorized, userIdResult.Errors.ToList());
        }
        var userId = userIdResult.Value;

        var isOwner = await formQueries.FormBelongsToCustomerAsync(request.FormId, userId, cancellationToken);

        if (!isOwner)
        {
            var error = ResultError.InvalidOperation("Form", "You can only access your own forms.");
            return ResultHelper.CreateFailureResponse<TResponse>(ResultType.Forbidden, new List<ResultError> { error });
        }
        return await next();
    }

}
