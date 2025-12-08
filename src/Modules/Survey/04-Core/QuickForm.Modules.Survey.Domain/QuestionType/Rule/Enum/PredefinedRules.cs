using System.Reflection;
using QuickForm.Common.Domain;
namespace QuickForm.Modules.Survey.Domain;

public static class PredefinedRules
{
    public static PredefinedRule Rule_Min =>
        new PredefinedRule(RuleType.Min, DataTypeType.IntType,
            "Specifies the minimum allowed numeric value.");

    public static PredefinedRule Rule_Max =>
        new PredefinedRule(RuleType.Max, DataTypeType.IntType,
            "Specifies the maximum allowed numeric value.");

    public static PredefinedRule Rule_MinLength =>
        new PredefinedRule(RuleType.MinLength, DataTypeType.IntType,
            "Defines the minimum number of characters required.");

    public static PredefinedRule Rule_MaxLength =>
        new PredefinedRule(RuleType.MaxLength, DataTypeType.IntType,
            "Defines the maximum number of characters allowed.");

    public static PredefinedRule Rule_Required =>
        new PredefinedRule(RuleType.Required, DataTypeType.BooleanType,
            "Indicates that this field is mandatory and cannot be left empty.");

    public static List<PredefinedRule> GetAll()
    {
        return typeof(PredefinedRules)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(PredefinedRule))
            .Select(p => (PredefinedRule)p.GetValue(null)!)
            .ToList();
    }
}

public class PredefinedRule
{
    public RuleType RuleType { get; set; }
    public DataTypeType DataTypeType { get; set; }
    public string Description { get; set; }
    public PredefinedRule(RuleType ruleType, DataTypeType dataTypeType, string description)
    {
        RuleType = ruleType;
        DataTypeType = dataTypeType;
        Description = description;
    }
    public override string ToString()
    {
        return $"{RuleType.GetName()}:{DataTypeType.GetName()}";
    }
}
