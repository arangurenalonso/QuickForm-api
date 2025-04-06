
using System.Reflection;

namespace QuickForm.Modules.Survey.Domain;
public static class PredefinedQuestionTypeAttributes
{
    public static QuestionTypeAttributeSeedConfig QuestionInputTypeText => new QuestionTypeAttributeSeedConfig(
        QuestionTypeType.InputTypeText,
        new List<AttributeQuestionTypeAttribute>
        {
            new AttributeQuestionTypeAttribute(Guid.Parse("2194295A-C550-49D9-9CE4-541347C2F21A"), AttributeType.Name, true),
            new AttributeQuestionTypeAttribute(Guid.Parse("2C1487CF-921D-42DA-9734-93D7076C534C"), AttributeType.Label, true),
            new AttributeQuestionTypeAttribute(Guid.Parse("E2F0D7CE-26E5-4881-8A23-E30BCA696908"), AttributeType.HelperText, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("C98EB09A-AD81-46D3-A0EB-F14395B30DC3"), AttributeType.Placeholder, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("C8AD7891-6FD7-42B6-8B7E-A9AF92692313"), AttributeType.InformationText, false),
        }
    );
    public static QuestionTypeAttributeSeedConfig QuestionInputTypeNumber => new QuestionTypeAttributeSeedConfig(
        QuestionTypeType.InputTypeNumber,
        new List<AttributeQuestionTypeAttribute>
        {
            new AttributeQuestionTypeAttribute(Guid.Parse("31F8B98F-90EA-45B5-8AF0-AE7A126B5603"), AttributeType.Name, true),
            new AttributeQuestionTypeAttribute(Guid.Parse("54AB1A43-0E16-49BA-8DED-19AE0BC7ACB7"), AttributeType.Label, true),
            new AttributeQuestionTypeAttribute(Guid.Parse("A02BD294-0FBD-4C6E-955E-BF7710877EAD"), AttributeType.HelperText, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("564466CA-0E44-4DE1-94E5-98AB39B0D1B8"), AttributeType.Placeholder, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("F7CB2D78-D0B8-4A28-960D-05C88BB10A56"), AttributeType.InformationText, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("DA574AF2-DF7A-4ABF-87D9-6424BBA5ED5C"), AttributeType.Prefix, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("BBDBDCB3-6BDE-4F5B-8A8D-16F629BE2A88"), AttributeType.Suffix, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("D1D55CE6-03BB-4364-9B39-2AE7F32929C4"), AttributeType.DecimalScale, false),
            new AttributeQuestionTypeAttribute(Guid.Parse("F1A91279-CF55-4889-826F-2F9123AC0E4A"), AttributeType.AllowNegative, false),
        }
    );
    public static List<QuestionTypeAttributeSeedConfig> GetAll()
    {
        return typeof(PredefinedQuestionTypeAttributes)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(QuestionTypeAttributeSeedConfig))
            .Select(p => (QuestionTypeAttributeSeedConfig)p.GetValue(null)!)
            .ToList();
    }
}
public class QuestionTypeAttributeSeedConfig
{
    public QuestionTypeType QuestionTypeType { get; set; }
    public List<AttributeQuestionTypeAttribute> AttributeQuestionTypeAttribute { get; set; } 
    public QuestionTypeAttributeSeedConfig(QuestionTypeType questionTypeType, List<AttributeQuestionTypeAttribute> attributeQuestionTypeAttribute)
    {
        QuestionTypeType = questionTypeType;
        AttributeQuestionTypeAttribute = attributeQuestionTypeAttribute;
    }

}
public class AttributeQuestionTypeAttribute
{
    public Guid IdRelation { get; set; }
    public AttributeType AttributeType { get; set; }
    public bool IsRequired { get; set; }
    public AttributeQuestionTypeAttribute (Guid idRelation, AttributeType attributeType, bool isRequired)
    {
        IdRelation = idRelation;
        AttributeType = attributeType;
        IsRequired = isRequired;
    }
}
