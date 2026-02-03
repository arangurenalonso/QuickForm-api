using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using System.Globalization;
using System.Text.Json;

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
}
