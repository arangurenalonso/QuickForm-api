namespace QuickForm.Common.Domain;

public static class ResultHelper
{
    public static TResponse CreateFailureResponse<TResponse>(
       ResultType type,
       List<ResultError> errors)
    {
        var responseType = typeof(TResponse);

        // ResultT<T>
        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(ResultT<>))
        {
            var innerType = responseType.GetGenericArguments()[0];

            var failureMethod = typeof(ResultT<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(ResultT<object>.FailureTListResultError));

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { type, errors })!;
            }
        }

        // Result
        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errors);
        }

        var errorMessage = string.Join(", ", errors.Select(e => e.Message));
        throw new InvalidOperationException(
            $"Unsupported response type: {responseType.FullName} | error: {errorMessage}");
    }
}
