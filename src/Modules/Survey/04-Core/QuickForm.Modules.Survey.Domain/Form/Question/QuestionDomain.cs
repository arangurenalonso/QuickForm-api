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
    public Result ApplyAttributeChanges(JsonElement properties, QuestionTypeDomain questionType)
    {
        if (IdQuestionType != questionType.Id)
        {
            return ResultError.InvalidOperation(
                "QuestionTypeMismatch",
                "The provided question type does not match the question's current type.");
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

            string? valueToStore;

            switch (prop.Value.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    valueToStore = null;
                    break;

                case JsonValueKind.String:
                    valueToStore = prop.Value.GetString();
                    break;

                case JsonValueKind.Number:
                    valueToStore = prop.Value.GetRawText();
                    break;

                case JsonValueKind.True:
                    valueToStore = "true";
                    break;

                case JsonValueKind.False:
                    valueToStore = "false";
                    break;

                case JsonValueKind.Array:
                case JsonValueKind.Object:
                    return ResultError.InvalidInput(
                        "Properties",
                        $"Attribute '{prop.Name}' does not support arrays or objects."
                    );

                default:
                    valueToStore = prop.Value.ToString();
                    break;
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
}
