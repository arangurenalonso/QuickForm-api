using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed record HttpMethodVO
{
    private static readonly HashSet<string> Allowed =
        new(StringComparer.Ordinal) { "GET", "POST", "PUT", "DELETE" };

    public string Value { get; }

    private HttpMethodVO(string value) => Value = value;
    private HttpMethodVO() { }

    public static ResultT<HttpMethodVO> Create(string? method)
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            return ResultError.EmptyValue("HttpMethod", "HTTP method cannot be null or empty.");
        }

        var s = method.Trim().ToUpperInvariant();

        if (!Allowed.Contains(s))
        {
            return ResultError.InvalidFormat(
                "HttpMethod",
                "Only GET, POST, PUT, and DELETE are allowed."
            );
        }

        return new HttpMethodVO(s);
    }

    public static readonly HttpMethodVO GET = new("GET");
    public static readonly HttpMethodVO POST = new("POST");
    public static readonly HttpMethodVO PUT = new("PUT");
    public static readonly HttpMethodVO DELETE = new("DELETE");

    public static implicit operator string(HttpMethodVO vo) => vo.Value;
    public override string ToString() => Value;
}
