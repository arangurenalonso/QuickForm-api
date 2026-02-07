using MediatR;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Application.Forms.Queries;

namespace QuickForm.Modules.Survey.Application;

public sealed class FormOwnershipBehavior<TRequest, TResponse>(
    ICurrentUserService currentUser,
    IFormQueries formQueries
) : IPipelineBehavior<TRequest, ResultT<TResponse>>
    where TRequest : IRequest<ResultT<TResponse>>
{
    public async Task<ResultT<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<ResultT<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request is not IRequireFormOwnership ownedRequest)
        {
            return await next();
        }

        var userIdResult = currentUser.UserId;
        if (userIdResult.IsFailure)
        {
            return ResultT<TResponse>.FailureT(ResultType.Unauthorized, userIdResult.Errors);
        }

        var userId = userIdResult.Value;

        var isOwner = await formQueries.FormBelongsToCustomerAsync(ownedRequest.FormId, userId, cancellationToken);

        if (!isOwner)
        {
            var error = ResultError.InvalidOperation("Form", "You can only access your own forms.");
            return ResultT<TResponse>.FailureT(ResultType.Forbidden, error);
        }

        return await next();
    }
}
