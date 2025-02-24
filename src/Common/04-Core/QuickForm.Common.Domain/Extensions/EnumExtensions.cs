using System.ComponentModel;

namespace QuickForm.Common.Domain;
public static class EnumExtensions
{
    public static Guid GetId(this Enum value)
    {
        var description = value.ToString();
        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo != null)
        {
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
        }
        if (Guid.TryParse(description, out var result))
        {
            return result;
        }
        else
        {
            return Guid.Empty;
        }
    }
    public static string GetDetail(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo != null)
        {
            var attributes = (DetailAttribute[])fieldInfo.GetCustomAttributes(typeof(DetailAttribute), false);

            if (attributes != null)
            {
                return attributes[0].Description;
            }
        }

        return value.ToString();
    }
}
