using System.Globalization;

namespace QuickForm.Modules.Survey.Domain;
public sealed record DateEnd
{
    public DateTime? Value { get; }
    private DateEnd(DateTime? value=null)
    {
        Value = value;
    }
     
    public static DateEnd WithRestriction() => new(DateTime.UtcNow.AddDays(30));
    public static DateEnd NoRestriction() => new DateEnd();
    public static DateEnd FromDatabase(DateTime? value)
    {
        return new DateEnd(value);
    }
    public bool HasNotExpired() => !Value.HasValue || Value > DateTime.UtcNow;
    public override string ToString()
    {
        return Value?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public static implicit operator string(DateEnd dateEnd) => dateEnd.ToString();
}
