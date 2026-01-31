using System.Text.Json;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionDomain : BaseDomainEntity<QuestionId>
{

    public FormSectionId IdFormSection { get; private set; }
    public QuestionTypeId IdQuestionType { get; private set; }
    public int SortOrder { get; private set; }
    #region One To Many
    public FormSectionDomain FormSection { get; private set; }
    public QuestionTypeDomain QuestionType { get; private set; }
    #endregion
    #region Many to One
    public ICollection<QuestionAttributeValueDomain> QuestionAttributeValue { get; private set; } = [];
    public ICollection<QuestionRuleValueDomain> QuestionRuleValue { get; private set; } = [];
    #endregion
    private QuestionDomain() { }
    private QuestionDomain(QuestionId id, FormSectionId idFormSection, QuestionTypeId idQuestionType, int sortOrder) : base(id)
    {
        IdFormSection = idFormSection;
        IdQuestionType = idQuestionType;
        SortOrder = sortOrder;
    }

    public static ResultT<QuestionDomain> Create(QuestionId id, FormSectionId idFormSection, QuestionTypeId idQuestionType, int order)
    {
        var newDomain = new QuestionDomain(id, idFormSection, idQuestionType, order);

        return newDomain;
    }
    public Result UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;

        return Result.Success();
    }

    public Result MoveToSection(FormSectionId newSectionId)
    {
        IdFormSection = newSectionId;
        return Result.Success();
    }

    public Result ApplyRuleChanges(Dictionary<string, ValidationRule>? rules, QuestionTypeDomain questionType)
    {
        if (IdQuestionType != questionType.Id)
        {
            return ResultError.InvalidOperation(
                "QuestionTypeMismatch",
                "The provided question type does not match the question's current type.");
        }

       var requiredRules = questionType.QuestionTypeRules
           .Where(x => x.IsRequired)
           .Select(x => x.Rule)
           .DistinctBy(r => r.KeyName.Value)
           .ToList();

        var hasRequiredRules = requiredRules.Count > 0;

        if (rules is null || rules.Count == 0)
        {
            if (hasRequiredRules)
            {
                return ResultError.InvalidInput(
                    "Rules",
                    $"Question type '{questionType.KeyName.Value}' has required rules; 'rules' cannot be null/undefined.");
            }

            foreach (var existing in QuestionRuleValue.Where(x => !x.IsDeleted))
            {
                existing.MarkDeleted();
            }

            return Result.Success();
        }

        var typeRuleByKey = questionType.QuestionTypeRules
            .ToDictionary(
                x => x.Rule.KeyName.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase);

        var existingValueByTypeRuleId = QuestionRuleValue
            .Where(x => !x.IsDeleted)
            .ToDictionary(x => x.IdQuestionTypeRule, x => x);

        var incomingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (ruleKey, incomingRule) in rules)
        {
            incomingKeys.Add(ruleKey);

            if (!typeRuleByKey.TryGetValue(ruleKey, out var typeRule))
            {
                return ResultError.InvalidInput(
                    "QuestionTypeRule",
                    $"The rule '{ruleKey}' is not defined for the question type '{questionType.KeyName.Value}'.");
            }

            if (!TryConvertScalar(ruleKey, incomingRule.Value, "Rules", out var valueToStore, out var convertError))
            {
                return convertError!;
            }

            if (typeRule.IsRequired && string.IsNullOrWhiteSpace(valueToStore))
            {
                return ResultError.InvalidInput(
                    "Rule",
                    $"Rule '{ruleKey}' on question type '{questionType.KeyName.Value}' must have a non-empty value.");
            }

            string messageToStore = string.IsNullOrWhiteSpace(incomingRule.MessageTemplate) 
                                            ? typeRule.Rule.DefaultValidationMessageTemplate.ValidationMessage
                                            : incomingRule.MessageTemplate;
            

            if (existingValueByTypeRuleId.TryGetValue(typeRule.Id, out var existing))
            {
                existing.Update(valueToStore, messageToStore);
            }
            else
            {
                var createdResult = QuestionRuleValueDomain.Create(
                    Id,
                    typeRule.Id,
                    valueToStore,
                    messageToStore);

                if (createdResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, createdResult.Errors);

                }

                QuestionRuleValue.Add(createdResult.Value);
            }
        }

        foreach (var rr in requiredRules)
        {
            if (!incomingKeys.Contains(rr.KeyName.Value))
            {
                return ResultError.InvalidInput(
                    "Rule",
                    $"Question type '{questionType.KeyName.Value}' requires rule '{rr.KeyName.Value}'.");
            }
        }

        var keyByTypeRuleId = questionType.QuestionTypeRules
            .ToDictionary(x => x.Id, x => x.Rule.KeyName.Value);

        foreach (var existing in QuestionRuleValue.Where(x => !x.IsDeleted))
        {
            if (!keyByTypeRuleId.TryGetValue(existing.IdQuestionTypeRule, out var key))
            {
                existing.MarkDeleted();
                continue;
            }

            if (!incomingKeys.Contains(key))
            {
                existing.MarkDeleted();
            }
        }

        return Result.Success();
    }

    public Result ApplyAttributeChanges(JsonElement properties, QuestionTypeDomain questionType)
    {
        if (IdQuestionType != questionType.Id)
        {
            return ResultError.InvalidOperation(
                "QuestionTypeMismatch",
                "The provided question type does not match the question's current type.");
        }
        var requiredAttributes = questionType.QuestionTypeAttributes
                                    .Where(x => x.IsRequired)
                                    .Select(x => x.Attribute)
                                    .DistinctBy(a => a.KeyName.Value)
                                    .ToList();

        var hasRequiredAttributes = requiredAttributes.Count > 0;

        if (properties.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            if (hasRequiredAttributes)
            {
                return ResultError.InvalidInput(
                    "Properties",
                    $"Question type '{questionType.KeyName.Value}' has required attributes; 'properties' cannot be null/undefined.");
            }

            foreach (var existing in QuestionAttributeValue.Where(x => !x.IsDeleted))
            {
                existing.MarkDeleted();
            }

            return Result.Success();
        }

        if (properties.ValueKind != JsonValueKind.Object)
        {
            return ResultError.InvalidInput(
                "Properties",
                "Properties must be a JSON object (e.g., { \"placeholder\": \"...\" }).");
        }

        var typeAttributeByKey = questionType.QuestionTypeAttributes
            .ToDictionary(
                x => x.Attribute.KeyName.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase);

        var existingValueByTypeAttrId = QuestionAttributeValue
            .Where(x => !x.IsDeleted) 
            .ToDictionary(x => x.IdQuestionTypeAttribute, x => x);

        var incomingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var prop in properties.EnumerateObject())
        {
            incomingKeys.Add(prop.Name);

            if (!typeAttributeByKey.TryGetValue(prop.Name, out var typeAttr))
            {
                return ResultError.InvalidInput(
                    "QuestionTypeAttribute",
                    $"The attribute '{prop.Name}' is not defined for the question type '{questionType.KeyName}'.");
            }


            if (!TryConvertScalar(prop.Name, prop.Value, "Properties", out var valueToStore, out var convertError))
            {
                return convertError!;
            }

            if (typeAttr.IsRequired && string.IsNullOrWhiteSpace(valueToStore))
            {
                return ResultError.InvalidInput(
                    "Properties",
                    $"Property '{prop.Name}' on question type '{questionType.KeyName.Value}' must have a non-empty value.");
            }

            if (existingValueByTypeAttrId.TryGetValue(typeAttr.Id, out var existing))
            {
                existing.Update(valueToStore);
            }
            else
            {
                var createdResult = QuestionAttributeValueDomain.Create(
                    Id,
                    typeAttr.Id,
                    valueToStore);

                if (createdResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, createdResult.Errors);
                }

                QuestionAttributeValue.Add(createdResult.Value);
            }
        }
        foreach (var ra in requiredAttributes)
        {
            if (!incomingKeys.Contains(ra.KeyName.Value))
            {
                return ResultError.InvalidInput(
                    "Properties",
                    $"Question type '{questionType.KeyName.Value}' requires property '{ra.KeyName.Value}'.");
            }
        }

        var keyByAttrId = questionType.QuestionTypeAttributes
                                .ToDictionary(x => x.Id, x => x.Attribute.KeyName.Value);


        foreach (var existing in QuestionAttributeValue.Where(x => !x.IsDeleted))
        {
            if (!keyByAttrId.TryGetValue(existing.IdQuestionTypeAttribute, out var key))
            {
                existing.MarkDeleted();
                continue;
            }

            if (!incomingKeys.Contains(key))
            {
                existing.MarkDeleted();
            }
        }


        return Result.Success();
    }

    private static bool TryConvertScalar(
        string key,
        JsonElement value,
        string payloadName,
        out string? valueToStore,
        out Result? error
)
    {
        valueToStore = null;
        error = null;

        switch (value.ValueKind)
        {
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                valueToStore = null;
                return true;

            case JsonValueKind.String:
                valueToStore = value.GetString();
                return true;

            case JsonValueKind.Number:
                valueToStore = value.GetRawText();
                return true;

            case JsonValueKind.True:
                valueToStore = "true";
                return true;

            case JsonValueKind.False:
                valueToStore = "false";
                return true;

            case JsonValueKind.Array:
            case JsonValueKind.Object:
                error = ResultError.InvalidInput(
                    payloadName,
                    $"Key '{key}' does not support arrays or objects.");
                return false;

            default:
                valueToStore = value.ToString();
                return true;
        }
    }
}
