namespace QuickForm.Common.Domain;

public sealed class StatusIconVO
{
    public string Value { get; }

    private StatusIconVO(string value)
    {
        Value = value;
    }

    // Lista de nombres de íconos válidos de lucide-react.
    // Amplía esta lista según los íconos que realmente uses en el front.
    private static readonly HashSet<string> AllowedIcons =
        new(StringComparer.Ordinal)
        {
            // básicos
            "AlertCircle",
            "AlertTriangle",
            "Check",
            "CheckCircle2",
            "Info",
            "X",
            "XCircle",
            "Circle",
            "Square",
            "MinusCircle",
            "PlusCircle",

            "Loader2",
            "Clock",
            "PauseCircle",
            "PlayCircle",
            "StopCircle",
            "RefreshCw",
            "Shield",
            "ShieldAlert",
            "ShieldCheck",
        };

    public static ResultT<StatusIconVO> Create(string? value)
    {
        if (value is null)
        {
            return ResultError.NullValue(nameof(StatusIconVO),
                "El icono no puede ser null. Debe ser el nombre de un icono de lucide-react.");
        }

        var trimmed = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return ResultError.EmptyValue(nameof(StatusIconVO),
                "El icono no puede ser vacío. Debe ser el nombre de un icono de lucide-react.");
        }

        // lucide-react usa PascalCase, respetamos el case para el front
        if (!AllowedIcons.Contains(trimmed))
        {
            return ResultError.InvalidInput(nameof(StatusIconVO),
                $"El icono '{trimmed}' no es válido. Debe ser un icono de lucide-react permitido.");
        }

        return new StatusIconVO(trimmed);
    }

    public override string ToString() => Value;
}
