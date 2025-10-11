using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickForm.Common.Domain;

public sealed record IdOrKeyName
{
    public RefKind Kind { get; }
    public Guid? Id { get; }
    public string? Name { get; }

    [MemberNotNullWhen(true, nameof(Id))]
    public bool IsById => Kind == RefKind.ById;

    [MemberNotNullWhen(true, nameof(Name))]
    public bool IsByKeyName => Kind == RefKind.ByKeyName;
    private IdOrKeyName(RefKind kind, Guid? id, string? name)
        => (Kind, Id, Name) = (kind, id, name);

    public static IdOrKeyName FromId(Guid id) => new(RefKind.ById, id, null);
    public static IdOrKeyName FromName(string name) => new(RefKind.ByKeyName, null, name);

    public static IdOrKeyName Parse(string value)
        => Guid.TryParse(value, out var gid) ? FromId(gid) : FromName(value);
}

public enum RefKind { ById, ByKeyName }
public sealed class IdOrNameRefJsonConverter : JsonConverter<IdOrKeyName>
{
    public override IdOrKeyName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var raw = reader.GetString()!;
            return IdOrKeyName.Parse(raw);
        }

        throw new JsonException($"Token inválido para IdOrNameRef: {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, IdOrKeyName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Kind == RefKind.ById
            ? value.Id!.Value.ToString()
            : value.Name);
    }
}
