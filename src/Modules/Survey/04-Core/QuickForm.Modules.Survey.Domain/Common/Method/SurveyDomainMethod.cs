using System.Globalization;
using System.Text.Json;
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


}
