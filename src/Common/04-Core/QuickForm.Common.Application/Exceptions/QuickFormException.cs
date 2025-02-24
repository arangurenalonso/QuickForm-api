using QuickForm.Common.Domain;

namespace QuickForm.Common.Application;
public sealed class QuickFormException : Exception
{
    public QuickFormException(string requestName, ResultError? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }

    public ResultError? Error { get; }

}
