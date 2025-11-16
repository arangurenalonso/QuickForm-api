using System.ComponentModel;
using System.Reflection;

namespace QuickForm.Common.Domain;
public static class EnumExtensions
{
    public static Guid GetId(this Enum value)
    {
        var type = value.GetType();
        var name = value.ToString();

        var fieldInfo = type.GetField(name);
        if (fieldInfo is null)
        {
            return Guid.Empty;
        }

        var attribute = fieldInfo.GetCustomAttribute<IdAttribute>(inherit: false);
        if (attribute is null)
        {
            return Guid.Empty;
        }

        return attribute.Value;
    }
    public static string GetName(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo != null)
        {
            var attributes = (NameAttribute[])fieldInfo.GetCustomAttributes(typeof(NameAttribute), false);

            if (attributes is { Length: > 0 } && attributes[0] != null)
            {
                return attributes[0].Description;
            }
        }

        return value.ToString();
    }
    public static string GetColor(this Enum value)
    {
        var type = value.GetType();
        var name = value.ToString();

        var fieldInfo = type.GetField(name);
        if (fieldInfo is null)
        {
            return string.Empty;
        }

        var attribute = fieldInfo.GetCustomAttribute<ColorAttribute>(inherit: false);
        if (attribute is null)
        {
            return string.Empty;
        }

        return attribute.Value;
    }
    public static string GetIcon(this Enum value)
    {
        var type = value.GetType();
        var name = value.ToString();

        var fieldInfo = type.GetField(name);
        if (fieldInfo is null)
        {
            return string.Empty;
        }

        var attribute = fieldInfo.GetCustomAttribute<IconAttribute>(inherit: false);
        if (attribute is null)
        {
            return string.Empty;
        }

        return attribute.Value;
    }
    public static TEnum? FromId<TEnum>(Guid id) where TEnum : struct, Enum
    {
        return Array.Find(Enum.GetValues<TEnum>(), t => t.GetId() == id);
    }
    public static TEnum? FromName<TEnum>(string detail) where TEnum : struct, Enum
    {
        return Array.Find(Enum.GetValues<TEnum>(), t =>
            string.Equals(t.GetName(), detail, StringComparison.OrdinalIgnoreCase));
    }

}
