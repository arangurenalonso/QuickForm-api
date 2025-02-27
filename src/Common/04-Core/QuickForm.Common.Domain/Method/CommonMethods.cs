namespace QuickForm.Common.Domain.Method;
public sealed class CommonMethods
{
    public static List<ResultError> ConvertExceptionToResult(Exception e,string field)
    {
        var listErrors = new List<ResultError>
                                    {
                                        ResultError.Exception(field, $"ErrorMessage: {e.Message}")
                                    };
        var inner = e.InnerException;
        int level = 1;
        while (inner is not null)
        {
            listErrors.Add(ResultError.Exception(field, $"InnerException-Level-{level}: {inner.Message}"));
            inner = inner.InnerException;
            level++;
        }

        return listErrors;
    }
}
