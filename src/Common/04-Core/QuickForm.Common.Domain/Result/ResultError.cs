namespace QuickForm.Common.Domain;
public record ResultError
{
    public string PropertyName { get; private set; } = "";
    public ErrorType Type { get; }
    public string Description { get; }

    public string Message =>
        string.IsNullOrWhiteSpace(PropertyName)
            ? $"{Description}"
            : $"{PropertyName}: {Description}";

    private ResultError(string field, string description, ErrorType type)
    {
        PropertyName = field;
        Type = type;
        Description = description;
    } 
    private ResultError(string description,ErrorType type)
    {
        Type = type;
        Description = description;
    }
    public static readonly ResultError None = new ResultError(string.Empty,ErrorType.None );


    public static ResultError NullValue(string field, string description) =>
        new(field, description, ErrorType.NullValue);
    public static ResultError EmptyValue(string field,string description ) =>
        new(field, description, ErrorType.EmptyValue);
    public static ResultError InvalidFormat(string field,string description) =>
        new(field, description, ErrorType.InvalidFormat);
    public static ResultError InvalidCharacter(string field, string description) =>
        new(field, description, ErrorType.InvalidCharacter);
    public static ResultError InvalidOperation(string field, string description) =>
        new(field, description, ErrorType.InvalidOperation);
    public static ResultError InvalidInput(string field, string description) =>
        new(field, description, ErrorType.InvalidInput);
    public static ResultError DuplicateValueAlreadyInUse(string field, string description) =>
        new(field, description, ErrorType.DuplicateValueAlreadyInUse);
    public static ResultError Exception(string field, string description) =>
        new(field, description, ErrorType.Exception);

    public void SetField(string field)
    {
        PropertyName = field;
    }
    public override string ToString()
       => string.IsNullOrWhiteSpace(PropertyName)
           ? $"{Type}: {Description}"
           : $"{PropertyName}: {Description} ({Type})";
}
