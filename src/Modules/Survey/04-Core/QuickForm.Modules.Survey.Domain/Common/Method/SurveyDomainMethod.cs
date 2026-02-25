using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public static class SurveyDomainMethod
{
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

    public static Result ValidateAttributes(JsonElement value, QuestionForSubmission question, string name)
    {
        // AllowNegative
        var allowNegativeAttr = question.Attributes.FirstOrDefault(a => a.AttributeId == AttributeType.AllowNegative.GetId());
        if (allowNegativeAttr.Value is not null)
        {
            if (!bool.TryParse(allowNegativeAttr.Value, out var allowNegative))
            {
                return ResultError.InvalidInput(
                    "FormDefinition",
                    $"Field '{name}' has AllowNegative attribute with invalid boolean value '{allowNegativeAttr.Value}'."
                );
            }

            if (!allowNegative && TryGetDecimal(value, out var dec) && dec < 0)
            {
                return ResultError.InvalidInput(name, "Negative values are not allowed.");
            }
        }

        // DecimalScale
        var decimalScaleAttr = question.Attributes.FirstOrDefault(a => a.AttributeId == AttributeType.DecimalScale.GetId());
        if (decimalScaleAttr.Value is not null)
        {
            if (!int.TryParse(decimalScaleAttr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var scale) || scale < 0)
            {
                return ResultError.InvalidInput(
                    "FormDefinition",
                    $"Field '{name}' has DecimalScale attribute with invalid integer value '{decimalScaleAttr.Value}'."
                );
            }

            if (TryGetDecimal(value, out var dec))
            {
                var decimals = CountDecimals(dec);
                if (decimals > scale)
                {
                    return ResultError.InvalidInput(name, $"Maximum allowed decimal places is {scale}.");
                }
            }
        }

        return Result.Success();
    }


    private static string GetMessageFromTemplateAndPlaceholder(string messageTemplate, string? placeholder, string valueToReplace)
    {
        if (string.IsNullOrEmpty(placeholder))
        {
            return messageTemplate;
        }

        return Regex.Replace(messageTemplate, placeholder, valueToReplace);
    }

    public static Result ValidateRules(
    JsonElement value,
    (Guid RuleId, string? Value, string Message, string? Placeholder) rule,
    QuestionId questionId,
    string name
)
    {

        switch (rule.RuleId)
        {
            case var r when r == RuleType.Required.GetId():
                {
                    if (string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!bool.TryParse(rule.Value, out var required))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has Required rule with invalid boolean value '{rule.Value}'."
                        );
                    }

                    if (required && IsEmpty(value))
                    {
                        var msg = GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, "");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.MaxLength.GetId():
                {
                    if (IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!int.TryParse(rule.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var maxLen))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has MaxLength rule with invalid int value '{rule.Value}'."
                        );
                    }

                    if (value.ValueKind != JsonValueKind.String)
                    {
                        return ResultError.InvalidInput(name, "The value must be a string.");
                    }

                    var s = value.GetString() ?? string.Empty;
                    if (s.Length > maxLen)
                    {
                        var msg = GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{maxLen}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.MinLength.GetId():
                {
                    if (IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!int.TryParse(rule.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var minLen))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has MinLength rule with invalid int value '{rule.Value}'."
                        );
                    }

                    if (value.ValueKind != JsonValueKind.String)
                    {
                        return ResultError.InvalidInput(name, "The value must be a string.");
                    }

                    var s = value.GetString() ?? string.Empty;
                    if (s.Length < minLen)
                    {
                        var msg = GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{minLen}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.Min.GetId():
                {
                    if (IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!decimal.TryParse(rule.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var min))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has Min rule with invalid decimal value '{rule.Value}'."
                        );
                    }

                    if (!TryGetDecimal(value, out var dec))
                    {
                        return ResultError.InvalidInput(name, "The value must be a valid number.");
                    }

                    if (dec < min)
                    {
                        var msg = GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{min}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.Max.GetId():
                {
                    if (IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }


                    if (!decimal.TryParse(rule.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var max))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has Max rule with invalid decimal value '{rule.Value}'."
                        );
                    }

                    if (!TryGetDecimal(value, out var dec))
                    {
                        return ResultError.InvalidInput(name, "The value must be a valid number.");
                    }

                    if (dec > max)
                    {
                        var msg = GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{max}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            default:
                return ResultError.InvalidInput(
                    "FormDefinition",
                    $"Validation for rule '{rule.RuleId}' is not implemented."
                );
        }
    }


    private static bool TryGetDecimal(JsonElement v, out decimal value)
    {
        value = default;

        if (v.ValueKind == JsonValueKind.Number)
        {
            return v.TryGetDecimal(out value);
        }

        if (v.ValueKind == JsonValueKind.String)
        {
            return decimal.TryParse(v.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        return false;
    }
    private static bool IsNull(JsonElement v)
        => v.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined;

    public static bool IsEmpty(JsonElement v)
        => v.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => true,
            JsonValueKind.String => string.IsNullOrWhiteSpace(v.GetString()),
            JsonValueKind.Array => v.GetArrayLength() == 0,
            _ => false
        };

    private static int CountDecimals(decimal d)
    {
        var bits = decimal.GetBits(d);
        return (bits[3] >> 16) & 0xFF;
    }
}
