using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using System.Globalization;
using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;
public static class CommonMethods
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

}
