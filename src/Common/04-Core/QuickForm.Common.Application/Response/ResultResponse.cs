namespace QuickForm.Common.Application;
public class ResultResponse
{
    public bool IsSuccess { get; }
    public string? Message { get; }

    protected ResultResponse(bool isSuccess, string? message = null)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static ResultResponse Success() => new ResultResponse(true);
    public static ResultResponse Success(string message) => new ResultResponse(true, message);
    public static ResultResponse Error(string message) => new ResultResponse(false, message);

}
