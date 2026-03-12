using System.Globalization;
using System.Text.Json;

namespace QuickForm.Common.Domain.Method;
public sealed class CommonJsonElementMethods
{
    public static bool TryParseRawValueToJsonElement(string rawElement, out JsonElement jsonElement)
    {
        jsonElement = default;

        if (string.IsNullOrWhiteSpace(rawElement))
        {
            return false;
        }

        rawElement = rawElement.Trim();

        try
        {
            var c = rawElement[0];

            var looksJson =
                c == '{' || c == '[' || c == '"' ||
                c == '-' || char.IsDigit(c) ||
                rawElement.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                rawElement.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                rawElement.Equals("null", StringComparison.OrdinalIgnoreCase);

            if (!looksJson)
            {
                return false;
            }

            using var doc = JsonDocument.Parse(rawElement);
            jsonElement = doc.RootElement.Clone();
            return true;
        }
        catch
        {
            jsonElement = default;
            return false;
        }
    }

    public static bool TryGetInt64(JsonElement jsonElement, out long value)
    {
        value = default;

        if (jsonElement.ValueKind == JsonValueKind.Number)
        {
            return jsonElement.TryGetInt64(out value);
        }

        if (jsonElement.ValueKind == JsonValueKind.String)
        {
            return long.TryParse(
                jsonElement.GetString(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out value);
        }

        return false;
    }
    public static bool TryGetBoolean(JsonElement jsonElement, out bool value)
    {
        value = default;

        if (jsonElement.ValueKind == JsonValueKind.True)
        {
            value = true;
            return true;
        }

        if (jsonElement.ValueKind == JsonValueKind.False)
        {
            value = false;
            return true;
        }

        if (jsonElement.ValueKind == JsonValueKind.String)
        {
            return bool.TryParse(jsonElement.GetString(), out value);
        }

        return false;
    }
    public static bool TryGetDateTime(JsonElement jsonElement, out DateTime value)
    {
        value = default;

        if (jsonElement.ValueKind == JsonValueKind.String)
        {
            var s = jsonElement.GetString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            if (DateTimeOffset.TryParse(
                s,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var dto))
            {
                value = dto.UtcDateTime;
                return true;
            }

            if (DateTime.TryParse(
                s,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var dt))
            {
                value = EnsureUtc(dt);
                return true;
            }
        }

        return false;
    }
    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    public static bool TryGetDecimal(JsonElement jsonElement, out decimal value)
    {
        value = default;

        if (jsonElement.ValueKind == JsonValueKind.Number)
        {
            return jsonElement.TryGetDecimal(out value);
        }

        if (jsonElement.ValueKind == JsonValueKind.String)
        {
            return decimal.TryParse(
                jsonElement.GetString(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value);
        }

        return false;
    }

    public static bool TryGetDateOnlyAsDateTime(JsonElement jsonElement, out DateTime value)
    {
        value = default;

        var s = jsonElement.ValueKind == JsonValueKind.String
            ? jsonElement.GetString()
            : jsonElement.GetRawText();

        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        if (DateOnly.TryParseExact(
            s,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var dateOnly))
        {
            value = dateOnly.ToDateTime(TimeOnly.MinValue);
            return true;
        }

        return false;
    }

    public static bool TryGetTimeOnly(JsonElement jsonElement, out TimeOnly value)
    {
        value = default;

        var s = jsonElement.ValueKind == JsonValueKind.String
            ? jsonElement.GetString()
            : jsonElement.GetRawText();

        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        return TimeOnly.TryParse(
            s,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out value);
    }


    public static bool IsNull(JsonElement v)
        => v.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined;

    public static bool IsEmpty(JsonElement v)
        => v.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => true,
            JsonValueKind.String => string.IsNullOrWhiteSpace(v.GetString()),
            JsonValueKind.Array => v.GetArrayLength() == 0,
            _ => false
        };

    public static bool IsNullOrUndefined(JsonElement? value)
    {
        return !value.HasValue ||
               value.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined;
    }
}
