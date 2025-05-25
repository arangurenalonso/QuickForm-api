using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using System.Globalization;
using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;
public static class SurveyCommonMethods
{
    public static Result ValidateDataType(string propertyName, string dataType, JsonElement value)
    {
        var dataTypeEnum = EnumExtensions.FromDetail<DataTypeType>(dataType);

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
            DataTypeType.BooleanType => value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False,
            DataTypeType.DatetimeType =>
                value.ValueKind == JsonValueKind.String
                    ? DateTime.TryParse(value.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                    : DateTime.TryParse(value.GetRawText(), CultureInfo.InvariantCulture, DateTimeStyles.None, out _),
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
    public static ResultT<object?> ConvertValue(string? valueToConvert, string dataType)
    {
        if (valueToConvert == null)
        {
            return null;
        }

        var dataTypeEnum = EnumExtensions.FromDetail<DataTypeType>(dataType);

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
