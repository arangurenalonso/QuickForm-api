using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;


public record ExpirationDate
{
    public DateTime Value { get; }

    private ExpirationDate(DateTime value)
    {
        Value = value;
    }

    public static ResultT<ExpirationDate> Create(DateTime? expirationDate)
    {
        if (expirationDate is null)
        {
            return ResultError.EmptyValue("ExpirationDate", "Expiration cannot be null or empty.");
        }
        if (expirationDate.Value <= DateTime.UtcNow)
        {
            return ResultError.InvalidOperation("ExpirationDate", "Expiration date must be in the future.");
        }

        return new ExpirationDate(expirationDate.Value);
    }
    public static ExpirationDate FromDatabase(DateTime value)
    {
        return new ExpirationDate(value);
    }
    public bool IsExpired() => DateTime.UtcNow >= Value;

    public static implicit operator DateTime(ExpirationDate expirationDate) => expirationDate.Value;
}
