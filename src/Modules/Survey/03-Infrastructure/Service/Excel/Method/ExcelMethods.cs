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

        if (bool.TryParse(trimmed, out var b))
        {
            return b ? "Yes" : "No";

        }

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
        if (el.TryGetInt32(out var i) && (i == 0 || i == 1))
        {
            return i == 1 ? "Yes" : "No";
        }
        return el.GetRawText();
    }
}
