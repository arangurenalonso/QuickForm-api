using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
public static class SurveyCommonMethods
{
    public static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;

        }
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
    public static ResultT<object?> ConvertValue(string? valueToConvert, string dataType)
    {
        if (valueToConvert == null)
        {
            return null;
        }

        var dataTypeEnum = EnumExtensions.FromName<DataTypeType>(dataType);

        if (dataTypeEnum == null)
        {
            var errorUnknown = ResultError.InvalidInput(
                "DataType",
                $"Unknown data type '{dataType}'."
            );
            return ResultT<object?>.FailureT(ResultType.Conflict, errorUnknown);
        }
        object? convertedValue = null;
        ResultError? resultError = null;
        switch (dataTypeEnum)
        {
            case DataTypeType.StringType:
                convertedValue = valueToConvert;
                break;

            case DataTypeType.IntType:
                if (int.TryParse(valueToConvert, out var intVal))
                {
                    convertedValue = intVal;
                }
                else
                {
                    resultError = ResultError.InvalidInput("ValueConversion", $"'{valueToConvert}' is not a valid integer.");
                }
                break;
                    
            case DataTypeType.DecimalType:
                if (decimal.TryParse(valueToConvert, NumberStyles.Any, CultureInfo.InvariantCulture, out var decVal))
                {
                    convertedValue = decVal;
                }
                else
                {
                    resultError = ResultError.InvalidInput("ValueConversion", $"'{valueToConvert}' is not a valid decimal.");
                }
                break;
            case DataTypeType.BooleanType:
                if (bool.TryParse(valueToConvert, out var boolVal))
                {
                    convertedValue = boolVal;
                }
                else
                {
                    resultError = ResultError.InvalidInput("ValueConversion", $"'{valueToConvert}' is not a valid boolean.");
                }
                break;
            case DataTypeType.DatetimeType:
                if (DateTime.TryParse(valueToConvert, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtVal))
                {
                    convertedValue = dtVal;
                }
                else
                {
                    resultError = ResultError.InvalidInput("ValueConversion", $"'{valueToConvert}' is not a valid datetime.");
                }
                break;
            default:
                resultError = ResultError.InvalidInput("ValueConversion", $"Unsupported data type conversion '{dataTypeEnum}'.");
                break;
        }

        if(resultError is not null)
        {
            return ResultT<object?>.FailureT(
                                       ResultType.DomainValidation,
                                       resultError
                                   );
        }
        return convertedValue;
    }





    public static object? ConvertStoredRawValueInObject(string typeKey, string? valuesStored)
    {
        if (string.IsNullOrWhiteSpace(valuesStored))
        {
            return null;
        }

        valuesStored = valuesStored.Trim();

        if (valuesStored.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var qt = EnumExtensions.FromName<QuestionTypeType>(typeKey);

        if (qt == null)
        {
            throw new ArgumentException($"Unknown question type '{typeKey}'.", nameof(typeKey));
        }
        if (!CommonJsonElementMethods.TryParseRawValueToJsonElement(valuesStored, out var jsonElement))
        {
            return null;
        }

        switch (qt)
        {
            case QuestionTypeType.InputTypeText:
                if (jsonElement is { ValueKind: JsonValueKind.String })
                {
                    return jsonElement.GetString();
                }
                return StripQuotes(valuesStored);

            case QuestionTypeType.InputTypeInteger:
                if (jsonElement is { ValueKind: JsonValueKind.Number } )
                {
                    if(jsonElement.TryGetInt32(out var i32))
                    {
                        return i32;

                    }
                    if (jsonElement.TryGetInt64(out var i64))
                    {
                        return i64;

                    }
                }
                var rawInt = StripQuotes(valuesStored);

                if (int.TryParse(rawInt, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i32_))
                {
                    return i32_;
                }

                if (long.TryParse(rawInt, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i64_))
                {
                    return i64_;
                }

                return null;

            case QuestionTypeType.InputTypeDecimal:
                if (jsonElement is { ValueKind: JsonValueKind.Number } && jsonElement.TryGetDecimal(out var dec))
                {
                    return dec;
                }

                var rawDec = StripQuotes(valuesStored);
                if (decimal.TryParse(rawDec, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                {
                    return d;
                }
                return null;

            case QuestionTypeType.InputTypeBoolean:
                if (jsonElement is { ValueKind: JsonValueKind.True or JsonValueKind.False })
                {
                    return jsonElement.GetBoolean();
                }

                var rawBool = StripQuotes(valuesStored);
                if (bool.TryParse(rawBool, out var b))
                {
                    return b;
                }

                return null;
            case QuestionTypeType.InputTypeDatetime:
                {
                    var s = (jsonElement is { ValueKind: JsonValueKind.String })
                        ? jsonElement.GetString()
                        : StripQuotes(valuesStored);

                    if (string.IsNullOrWhiteSpace(s))
                    {
                        return null;
                    }
                    if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dto))
                    {
                        return dto; 
                    }

                    return null;
                }
            case QuestionTypeType.InputTypeDate:
                {
                    var s = (jsonElement is { ValueKind: JsonValueKind.String })
                        ? jsonElement.GetString()
                        : StripQuotes(valuesStored);

                    if (DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var da))
                    {
                        return da;
                    }

                    return null;
                }

            case QuestionTypeType.InputTypeTime:
                {
                    var s = (jsonElement is { ValueKind: JsonValueKind.String })
                        ? jsonElement.GetString()
                        : StripQuotes(valuesStored);

                    if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var t))
                    {
                        return t;
                    }

                    return null;
                }
            default:
                throw new ArgumentException($"Unknown question type '{typeKey}'.", nameof(typeKey));
        }
    }

    private static string StripQuotes(string raw)
    {
        if (raw.Length >= 2 && raw[0] == '"' && raw[^1] == '"')
        {
            return raw.Substring(1, raw.Length - 2);
        }

        return raw;
    }
}
