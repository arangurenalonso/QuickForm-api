using Newtonsoft.Json;

namespace QuickForm.Common.Infrastructure;
public static class JsonPrototype
{
    /// <summary>
    /// Serializes an object to a JSON string using the default serializer settings.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string Serialize(object obj, JsonSerializerSettings? serializerSettings=null)
    {
        return JsonConvert.SerializeObject(obj, serializerSettings is null? SerializerSettings.DefaultInstance: serializerSettings);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of type T using the default serializer settings.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of type T.</returns>
    public static T Deserialize<T>(string json, JsonSerializerSettings? serializerSettings = null)
    {
        return JsonConvert.DeserializeObject<T>(json, serializerSettings is null ? SerializerSettings.DefaultInstance : serializerSettings)!;
    }

    /// <summary>
    /// Deserializes a JSON string to an object of a specified type using the default serializer settings.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <returns>An object of the specified type.</returns>
    public static object? Deserialize(string json, Type type, JsonSerializerSettings? serializerSettings = null)
    {
        return JsonConvert.DeserializeObject(json, type, serializerSettings is null ? SerializerSettings.DefaultInstance : serializerSettings);
    }
}
