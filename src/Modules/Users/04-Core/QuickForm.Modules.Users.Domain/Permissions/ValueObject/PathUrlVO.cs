using System.Text.RegularExpressions;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed record PathUrlVO
{

    private static readonly Regex ParamPatternSimple = new(@"^\{([A-Za-z][A-Za-z0-9_]*)\}$", RegexOptions.Compiled);
    public string Value { get; }

    private PathUrlVO(string value) => Value = value;
    private PathUrlVO() { } 

    public static ResultT<PathUrlVO> Create(string? template)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            return ResultError.EmptyValue("RouteTemplate", "Route template cannot be null or empty.");
        }

        var normalized = Normalize(template);

        if (normalized[0] != '/')
        {
            return ResultError.InvalidFormat("RouteTemplate", "Route template must start with '/'.");
        }

        if (normalized.Length > 1 && normalized.EndsWith('/'))
        {
            return ResultError.InvalidFormat("RouteTemplate", "Trailing '/' is not allowed (except for root '/').");
        }

        if (normalized.Contains("//", StringComparison.Ordinal))
        {
            return ResultError.InvalidFormat("RouteTemplate", "Empty segments ('//') are not allowed.");
        }

        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var paramNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var seg in segments)
        {
            if (!seg.StartsWith('{') || !seg.EndsWith('}'))
            {
                if (!Regex.IsMatch(seg, @"^[A-Za-z0-9._-]+$"))
                {
                    return ResultError.InvalidFormat("RouteTemplate", $"Invalid literal segment '{seg}'.");
                }
                continue;
            }
            if (seg.Contains(':') || seg.Contains('?'))
            {
                return ResultError.InvalidFormat(
                    "RouteTemplate",
                    $"Only simple parameters are allowed: use '{{paramName}}' (found '{seg}').");
            }

            var m = ParamPatternSimple.Match(seg);
            if (!m.Success)
            {
                return ResultError.InvalidFormat("RouteTemplate", $"Invalid parameter segment '{seg}'. Expected '{{paramName}}'.");
            }

            var name = m.Groups[1].Value;
            if (!paramNames.Add(name))
            {
                return ResultError.InvalidFormat("RouteTemplate", $"Duplicate parameter name '{{{name}}}' is not allowed.");
            }
        }

        if (normalized.Contains("//", StringComparison.Ordinal))
        {
            return ResultError.InvalidFormat("RouteTemplate", "Empty segments ('//') are not allowed.");
        }

        return new PathUrlVO(normalized);
    }


    private static string Normalize(string s)
    {
        var trimmed = s.Trim();
        return trimmed;
    }

    public override string ToString() => Value;
    
    public static readonly PathUrlVO Root = new("/");
}
