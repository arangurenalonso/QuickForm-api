using System.Reflection;
using QuickForm.Common.Domain;
namespace QuickForm.Modules.Survey.Domain;

public static class PredefinedAttributes
{
    public static PredefinedAttribute Attribute_Name =>
        new PredefinedAttribute(AttributeType.Name, DataTypeType.StringType, "Internal identifier for the field, used to associate submitted values with corresponding keys in the data model. Must be unique within the form.");

    public static PredefinedAttribute Attribute_Label =>
        new PredefinedAttribute(AttributeType.Label, DataTypeType.StringType, "Label displayed to the user.");

    public static PredefinedAttribute Attribute_HelperText =>
        new PredefinedAttribute(AttributeType.HelperText, DataTypeType.StringType, "Short text to assist the user with input.");

    public static PredefinedAttribute Attribute_Placeholder =>
        new PredefinedAttribute(AttributeType.Placeholder, DataTypeType.StringType, "Placeholder shown inside the input field.");

    public static PredefinedAttribute Attribute_InformationText =>
        new PredefinedAttribute(AttributeType.InformationText, DataTypeType.StringType, "Additional informational text shown near the input.");

    public static PredefinedAttribute Attribute_Prefix =>
        new PredefinedAttribute(AttributeType.Prefix, DataTypeType.StringType, "Text shown before the input value.");

    public static PredefinedAttribute Attribute_Suffix =>
        new PredefinedAttribute(AttributeType.Suffix, DataTypeType.StringType, "Text shown after the input value.");

    public static PredefinedAttribute Attribute_DecimalScale =>
        new PredefinedAttribute(AttributeType.DecimalScale, DataTypeType.IntType, "Number of digits allowed after the decimal point.");

    public static PredefinedAttribute Attribute_AllowNegative =>
        new PredefinedAttribute(AttributeType.AllowNegative, DataTypeType.BooleanType, "Specifies if negative numbers are allowed.");

    public static List<PredefinedAttribute> GetAll()
    {
        return typeof(PredefinedAttributes)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(PredefinedAttribute))
            .Select(p => (PredefinedAttribute)p.GetValue(null)!)
            .ToList();
    }
}

public class PredefinedAttribute
{
    public AttributeType AttributeType { get; set; }
    public DataTypeType DataTypeType { get; set; }
    public string Description { get; set; }
    public PredefinedAttribute(AttributeType attributeType, DataTypeType dataTypeType,string description)
    {
        AttributeType = attributeType;
        DataTypeType = dataTypeType;
        Description = description;
    }
    public override string ToString()
    {
        return $"{AttributeType.GetDetail()}:{DataTypeType.GetDetail()}";
    }
}
