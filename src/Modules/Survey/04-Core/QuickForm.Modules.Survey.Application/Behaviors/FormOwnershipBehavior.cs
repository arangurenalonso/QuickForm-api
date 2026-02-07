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
            return CreateFailure(ResultType.Unauthorized, userIdResult.Errors.ToList());
        }
        var userId = userIdResult.Value;

        var isOwner = await formQueries.FormBelongsToCustomerAsync(request.FormId, userId, cancellationToken);

        if (!isOwner)
        {
            var error = ResultError.InvalidOperation("Form", "You can only access your own forms.");
            return CreateFailure(ResultType.Forbidden, new List<ResultError> { error });
        }
        return await next();
    }

    private TResponse CreateFailure(ResultType type, List<ResultError> errors)
    {
        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(ResultT<>))
        {
            var innerType = typeof(TResponse).GetGenericArguments()[0];

            var failureMethod = typeof(ResultT<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(ResultT<object>.FailureTListResultError));

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { type, errors })!;
            }
        }

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errors);
        }

        throw new InvalidOperationException($"Unsupported response type: {typeof(TResponse).FullName}");
    }
}
