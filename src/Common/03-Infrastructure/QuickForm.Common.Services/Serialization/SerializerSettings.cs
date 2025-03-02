using Newtonsoft.Json;

namespace QuickForm.Common.Infrastructure;
public static class SerializerSettings
{
    public static readonly JsonSerializerSettings DefaultInstance = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };
    public static readonly JsonSerializerSettings CleanInstance = new()
    {
        TypeNameHandling = TypeNameHandling.None, // ✅ Evita agregar metadatos innecesarios
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore, // Opcional: Ignorar valores nulos
        Formatting = Formatting.None // Opcional: Para evitar espacios innecesarios
    };
}
