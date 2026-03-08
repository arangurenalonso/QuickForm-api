using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public static class SurveyDomainMethod
{
    public static string FormatDecimalDisplayPreservingScale(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        rawValue = rawValue.Trim();

        if (rawValue.StartsWith('"') && rawValue.EndsWith('"') && rawValue.Length >= 2)
        {
            rawValue = rawValue[1..^1];
        }

        var isNegative = rawValue.StartsWith('-');
        if (isNegative)
        {
            rawValue = rawValue[1..];
        }

        var parts = rawValue.Split('.', 2);
        var integerPartRaw = parts[0];
        var decimalPartRaw = parts.Length > 1 ? parts[1] : null;

        if (!long.TryParse(integerPartRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integerPart))
        {
            return isNegative ? $"-{rawValue}" : rawValue;
        }

        var formattedIntegerPart = integerPart.ToString("#,##0", CultureInfo.GetCultureInfo("en-US"));

        var result = decimalPartRaw is not null
            ? $"{formattedIntegerPart}.{decimalPartRaw}"
            : formattedIntegerPart;

        return isNegative ? $"-{result}" : result;
    }


    public static bool TryConvertScalar(
            string key,
            JsonElement value,
            string payloadName,
            out string? valueToStore,
            out Result? error
    )
    {
        valueToStore = null;
        error = null;

        switch (value.ValueKind)
        {
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                valueToStore = null;
                return true;

            case JsonValueKind.String:
                valueToStore = value.GetString();
                return true;

            case JsonValueKind.Number:
                valueToStore = value.GetRawText();
                return true;

            case JsonValueKind.True:
                valueToStore = "true";
                return true;

            case JsonValueKind.False:
                valueToStore = "false";
                return true;

            case JsonValueKind.Array:
            case JsonValueKind.Object:
                error = ResultError.InvalidInput(
                    payloadName,
                    $"Key '{key}' does not support arrays or objects.");
                return false;

            default:
                valueToStore = value.ToString();
                return true;
        }
    }

    public static Result ValidateDataType(string propertyName, string dataType, JsonElement value)
    {
        if (string.IsNullOrWhiteSpace(dataType))
        {
            var errorUnknown = ResultError.InvalidInput(
                "DataType",
                $"Missing data type for attribute '{propertyName}'."
            );
            return Result.Failure(ResultType.MismatchValidation, errorUnknown);
        }
        var normalizedTypeName = dataType.Trim().ToLowerInvariant();
        var dataTypeEnum = EnumExtensions.FromName<DataTypeType>(normalizedTypeName);

        if (dataTypeEnum == null)
        {
            var errorUnknown = ResultError.InvalidInput(
                "DataType",
                $"Unknown data type '{dataType}' for attribute '{propertyName}'."
            );
            return Result.Failure(ResultType.MismatchValidation, errorUnknown);
        }

        bool isValid = dataTypeEnum switch
        {
            DataTypeType.StringType => value.ValueKind == JsonValueKind.String,

            DataTypeType.IntType => value.TryGetInt32(out _),

            DataTypeType.DecimalType => value.TryGetDecimal(out _),

            DataTypeType.BooleanType =>
                value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False,

            DataTypeType.DatetimeType =>
                value.ValueKind == JsonValueKind.String
                    ? DateTime.TryParse(
                        value.GetString(),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out _)
                    : DateTime.TryParse(
                        value.GetRawText(),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out _),

            _ => false
        };

        if (!isValid)
        {
            var error = ResultError.InvalidInput(
                "DataType",
                $"Invalid data type for attribute '{propertyName}'. Expected '{dataType}', but received '{value.ValueKind}'."
            );
            return Result.Failure(ResultType.DomainValidation, error);
        }

        return Result.Success();
    }

    public static ResultT<Dictionary<string, JsonElement>> NormalizeRequest(IReadOnlyDictionary<string, JsonElement> request)
    {
        var normalized = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);

        foreach (var (rawKey, value) in request)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
            {
                var err = ResultError.InvalidInput("Request", "Request contains an empty key.");
                return ResultT<Dictionary<string, JsonElement>>.FailureT(ResultType.DomainValidation, err);
            }

            var key = rawKey.Trim();

            if (!normalized.TryAdd(key, value))
            {
                var err = ResultError.InvalidInput(
                    "Request",
                    $"Duplicate field '{key}' in request (case-insensitive duplicates are not allowed)."
                );
                return ResultT<Dictionary<string, JsonElement>>.FailureT(ResultType.DomainValidation, err);
            }
        }

        return normalized;
    }

    
    public static string GetMessageFromTemplateAndPlaceholder(string messageTemplate, string? placeholder, string valueToReplace)
    {
        if (string.IsNullOrEmpty(placeholder))
        {
            return messageTemplate;
        }

        return Regex.Replace(messageTemplate, placeholder, valueToReplace);
    }
    
    public static int CountDecimals(decimal d)
    {
        var bits = decimal.GetBits(d);
        return (bits[3] >> 16) & 0xFF;
    }
}
