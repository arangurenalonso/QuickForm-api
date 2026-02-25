using System.Text.Json;

namespace QuickForm.Modules.Survey.Service;

internal static class ExcelValueFormatter
{
    public static string ToExcelText(string rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return "";
        }

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var el = doc.RootElement;

            return el.ValueKind switch
            {
                JsonValueKind.True => "Yes",
                JsonValueKind.False => "No",
                JsonValueKind.Null or JsonValueKind.Undefined => "",
                JsonValueKind.String => FromString(el.GetString()),
                JsonValueKind.Number => FromNumber(el),
                JsonValueKind.Array or JsonValueKind.Object => el.GetRawText(), // keep JSON
                _ => el.ToString() ?? ""
            };
        }
        catch
        {
            // If rawJson is not valid JSON, return as-is
            return rawJson;
        }
    }

    private static string FromString(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return "";
        }

        var trimmed = s.Trim();

        // handle "true"/"false"
        if (bool.TryParse(trimmed, out var b))
        {
            return b ? "Yes" : "No";

        }

        // handle "1"/"0"
        if (trimmed == "1")
        {
            return "Yes";
        }
        if (trimmed == "0")
        {
            return "No";
        }

        return trimmed;
    }

    private static string FromNumber(JsonElement el)
    {
        // handle 0/1 as boolean
        if (el.TryGetInt32(out var i) && (i == 0 || i == 1))
        {
            return i == 1 ? "Yes" : "No";
        }

        // keep number as text
        return el.GetRawText();
    }
}
