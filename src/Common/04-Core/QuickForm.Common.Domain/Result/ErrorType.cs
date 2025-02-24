namespace QuickForm.Common.Domain;
public enum ErrorType
{
    None=0,
    NullValue= 1,
    EmptyValue=2,
    InvalidFormat = 3,
    InvalidCharacter = 4,
    InvalidInput = 5,
    InvalidOperation = 6,
    DuplicateValueAlreadyInUse = 7,
    Exception = 99
}
