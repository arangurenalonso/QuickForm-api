namespace QuickForm.Common.Domain;
public class Result
{
    public ResultType ResultType { get; private set; } = ResultType.Unspecified;
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ResultErrorList Errors { get; } = new ResultErrorList();

    protected Result(bool isSuccess, ResultError error, ResultType? resultType = null)
    {
        SetResultType(isSuccess, resultType);
        if (isSuccess && error != ResultError.None)
        {
            throw new ArgumentException("Invalid Error", nameof(error));
        }

        if (!isSuccess && error == ResultError.None)
        {
            throw new ArgumentException("Invalid Error", nameof(error));
        }

        IsSuccess = isSuccess;
        if (error != ResultError.None)
        {
            Errors.Add(error);
        }
    }
    protected Result(bool isSuccess, List<ResultError> errors, ResultType? resultType = null)
    {

        SetResultType(isSuccess, resultType);
        if (isSuccess && errors.Any())
        {
            throw new ArgumentException("Invalid Error", nameof(errors));
        }

        if (!isSuccess && !errors.Any())
        {
            throw new ArgumentException("Invalid Error", nameof(errors));
        }

        IsSuccess = isSuccess;
        Errors = new ResultErrorList(errors);
    }
    protected Result(bool isSuccess, ResultErrorList erros, ResultType? resultType = null)
    {
        SetResultType(isSuccess, resultType);

        if (isSuccess && erros.Count > 0)
        {
            throw new ArgumentException("Invalid Error", nameof(erros));
        }

        if (!isSuccess && erros.Count == 0)
        {
            throw new ArgumentException("Invalid Error", nameof(erros));
        }

        IsSuccess = isSuccess;
        Errors = erros;
    }

    private void SetResultType(bool isSuccess, ResultType? resultType = null)
    {
        if (isSuccess)
        {
            ResultType = ResultType.None;
        }
        else
        {
            if (resultType is not null)
            {
                ResultType = resultType.Value;
            }
        }

    }

    public static implicit operator Result(ResultError error) => Failure(error);
    public static implicit operator Result(List<ResultError> error) => Failure(error);
    public static implicit operator Result(ResultErrorList error) => Failure(error);
    public static Result Success() => new(true, ResultError.None);
    public static Result Failure(ResultError error) => new(false, error, null);
    public static Result Failure(List<ResultError> errors) => new(false, errors, null);
    public static Result Failure(ResultErrorList errors) => new(false, errors, null);
    public static Result Failure(ResultType resultType,ResultError error ) => new(false, error, resultType);
    public static Result Failure(ResultType resultType,List<ResultError> errors) => new(false, errors, resultType);
    public static Result Failure(ResultType resultType,ResultErrorList errors) => new(false, errors, resultType);


}

