using System.Diagnostics.CodeAnalysis;

namespace QuickForm.Common.Domain;
public class ResultT<TValue> : Result
{
    private readonly TValue? _value;

    protected internal ResultT(TValue value) : base(true, ResultError.None)
    {
        _value = value;
    }
    protected internal ResultT(ResultError error, ResultType? resultType = null) : base(false, error, resultType)
    {
        _value = default;
    }
    protected internal ResultT(List<ResultError> errors, ResultType? resultType = null) : base(false, errors, resultType)
    {
        _value = default;
    }
    protected internal ResultT(ResultErrorList errors, ResultType? resultType = null) : base(false, errors, resultType)
    {
        _value = default;
    }
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");
    

    public static implicit operator ResultT<TValue>(TValue value) => new ResultT<TValue>(value);
    public static implicit operator ResultT<TValue>(ResultError error) => new ResultT<TValue>(error);
    public static implicit operator ResultT<TValue>(List<ResultError> errors) => new ResultT<TValue>(errors);
    public static implicit operator ResultT<TValue>(ResultErrorList errors) => new ResultT<TValue>(errors);

    public static ResultT<TValue> Success(TValue value) => new(value);
    public static ResultT<TValue> FailureT(ResultError error) => new(error, null);
    public static ResultT<TValue> FailureT(List<ResultError> errors) => new(errors, null);
    public static ResultT<TValue> FailureT(ResultErrorList errors) => new(errors, null);
    public static ResultT<TValue> FailureT(ResultType resultType, ResultError error) => new(error, resultType);
    public static ResultT<TValue> FailureTListResultError(ResultType resultType, List<ResultError> errors) => new(errors, resultType);
    public static ResultT<TValue> FailureT(ResultType resultType, List<ResultError> errors) => new(errors, resultType);
    public static ResultT<TValue> FailureT(ResultType resultType, ResultErrorList errors) => new(errors, resultType);


}
