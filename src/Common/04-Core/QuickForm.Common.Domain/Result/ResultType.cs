namespace QuickForm.Common.Domain;
public enum ResultType
{
    None = 0,
    NullValue=2,
    Unspecified = 3,
    BadRequest = 9,
    ModelDataValidation = 10,
    FluentValidation = 11,
    DomainValidation = 12,
    MismatchValidation = 13,
    NotFound = 21,
    Conflict = 20,
    InternalServerError = 22,
    Unauthorized = 23,
    Forbidden = 24,
    DataBaseTransaction=30,
    UnexpectedError=31,
}
