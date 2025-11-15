namespace QuickForm.Common.Domain;

public sealed class StatusColorVO
{
    public string Value { get; }

    private StatusColorVO(string value)
    {
        Value = value;
    }

    private static readonly HashSet<string> AllowedColors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "default",
            "info",
            "success",
            "warning",
            "error",
            "primary",
            "secondary",
            "neutral"
        };

    public static StatusColorVO Default => new StatusColorVO("default");
    public static ResultT<StatusColorVO> Create(string value)
    {
        if (value is null)
        {
            return ResultError.NullValue(nameof(StatusColorVO),
                "El color de estado no puede ser null.");
        }

        var trimmed = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return ResultError.EmptyValue(nameof(StatusColorVO),
                "El color de estado no puede ser vacío.");
        }

        if (!AllowedColors.Contains(trimmed))
        {
            return ResultError.InvalidInput(nameof(StatusColorVO),
                $"El color '{trimmed}' no es válido. Valores permitidos: {string.Join(", ", AllowedColors)}.");
        }

        return new StatusColorVO(trimmed.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
