namespace QuickForm.Common.Application;
public class ResultTResponse<T> : ResultResponse
{
    public  T? Data { get; }

    private ResultTResponse(bool isSuccess, string? message = null, T? data = default)
        : base(isSuccess, message)
    {
        Data = data;
    }

    public static ResultTResponse<T> Success(T data) => new ResultTResponse<T>(true, null, data);
    public static ResultTResponse<T> Success(T data, string message) => new ResultTResponse<T>(true, message, data);
}
