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
    public static string GetDetail(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo != null)
        {
            var attributes = (NameAttribute[])fieldInfo.GetCustomAttributes(typeof(NameAttribute), false);

            if (attributes != null)
            {
                return attributes[0].Description;
            }
        }

        return value.ToString();
    }
    public static TEnum? FromDetail<TEnum>(string detail) where TEnum : struct, Enum
    {
        return Array.Find(Enum.GetValues<TEnum>(), t =>
            string.Equals(t.GetDetail(), detail, StringComparison.OrdinalIgnoreCase));
    }

}
