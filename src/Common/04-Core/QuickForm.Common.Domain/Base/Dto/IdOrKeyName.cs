using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickForm.Common.Domain;

public sealed record IdOrKeyName
{
    public RefKind Kind { get; }
    public MasterId? Id { get; }
    public KeyNameVO? Name { get; }

    [MemberNotNullWhen(true, nameof(Id))]
    public bool IsById => Kind == RefKind.ById;

    [MemberNotNullWhen(true, nameof(Name))]
    public bool IsByKeyName => Kind == RefKind.ByKeyName;
    private IdOrKeyName(RefKind kind, MasterId? id, KeyNameVO? name)
        => (Kind, Id, Name) = (kind, id, name);

    public static IdOrKeyName FromId(MasterId id) => new(RefKind.ById, id, null);
    public static IdOrKeyName FromName(KeyNameVO name) => new(RefKind.ByKeyName, null, name);
    public static IdOrKeyName FromId(Guid id) => FromId(new MasterId(id));
    public static IdOrKeyName FromName(string name)
    {
        var r = KeyNameVO.Create(name);
        if (r.IsFailure)
        {
            throw new ArgumentException(r.Errors.ToString(), nameof(name));
        }
        return FromName(r.Value);
    }
    public static IdOrKeyName Parse(string value)
    {
        if (Guid.TryParse(value, out var gid))
        {
            return FromId(gid);
        }

        var res = KeyNameVO.Create(value);
        if (res.IsFailure)
        {
            throw new FormatException($"Invalid KeyName: {res.Errors.ToString()}");
        }

        return FromName(res.Value);
    }
}

public enum RefKind { ById, ByKeyName }
public sealed class IdOrNameRefJsonConverter : JsonConverter<IdOrKeyName>
{
    public override IdOrKeyName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var raw = reader.GetString()!;
            try
            {
                return IdOrKeyName.Parse(raw);
            }
            catch (FormatException ex)
            {
                throw new JsonException($"Invalid IdOrKeyName value: {raw}. {ex.Message}", ex);
            }
        }

        throw new JsonException($"Invalid token for IdOrKeyName: {reader.TokenType}. Expected string.");
    }

    public override void Write(Utf8JsonWriter writer, IdOrKeyName value, JsonSerializerOptions options)
    {
        if (value.Kind == RefKind.ById)
        {
            writer.WriteStringValue(value.Id!.Value.ToString());
        }
        else
        {
            writer.WriteStringValue(value.Name!.Value);
        }
    }
}
