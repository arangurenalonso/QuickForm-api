using System.Globalization;
using System.Text.Json;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionValidationService
{
    public Result Validate(
            IReadOnlyList<QuestionToValidate> questions,
            IReadOnlyList<QuestionTypeDomain> questionsType
        )
    {
        var questionTypeByKey = new Dictionary<string, QuestionTypeDomain>(StringComparer.OrdinalIgnoreCase);

        foreach (var qt in questionsType)
        {
            if (!questionTypeByKey.TryAdd(qt.KeyName.Value, qt))
            {
                return Result.Failure(
                    ResultType.ModelDataValidation,
                    ResultError.InvalidInput(
                        "QuestionType",
                        $"Duplicate QuestionType keyName detected: '{qt.KeyName.Value}'."
                    )
                );
            }
        }


        foreach (var question in questions)
        {
            if (!questionTypeByKey.TryGetValue(question.Type, out var questionType))
            {
                var errorQuestion = ResultError.InvalidInput(
                    "QuestionType",
                    $"Question type not found: {question.Type}."
                );

                return Result.Failure(ResultType.NotFound, errorQuestion);

            }

            var validateAttributeResult = ValidateAttribute(questionType, question);
            if (validateAttributeResult.IsFailure)
            {
                return validateAttributeResult;
            }

            var validateRulesResult = ValidateRules(questionType, question);
            if (validateRulesResult.IsFailure)
            {
                return validateRulesResult;
            }
        }

        var resultUniqueAttributeAcrossForm = ValidateUniqueAttributeAcrossForm(questions, questionsType);
        if (resultUniqueAttributeAcrossForm.IsFailure)
        {
            return resultUniqueAttributeAcrossForm;
        }

        return Result.Success();
    }
    private Result ValidateAttribute(
        QuestionTypeDomain questionTypeDomain,
        QuestionToValidate question
        )
    {
        if (question.Properties.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure(
                ResultType.ModelDataValidation,
                ResultError.InvalidInput(
                    "Properties",
                    $"Question '{question.Id}' of type '{question.Type}' must include 'properties' as a JSON object."
                )
            );
        }
        var props = ToPropertyMap(question.Properties);
        var resultValidateRequiredProperties = ValidateRequiredProperties(questionTypeDomain, props);
        if (resultValidateRequiredProperties.IsFailure)
        {
            return resultValidateRequiredProperties;
        }

        var validatePropertiesResult = ValidateDataTypesOfProperties(questionTypeDomain, props);
        if (validatePropertiesResult.IsFailure)
        {
            return validatePropertiesResult;
        }
        return Result.Success();
    }
    private Result ValidateRules(
        QuestionTypeDomain questionTypeDomain,
        QuestionToValidate question
    )
    {
        var requiredRules = questionTypeDomain.QuestionTypeRules
            .Where(x => x.IsRequired)
            .Select(x => x.Rule)
            .DistinctBy(r => r.KeyName.Value)
            .ToList();

        var hasRequiredRules = requiredRules.Count > 0;

        if (question.Rules is null || question.Rules.Count == 0)
        {
            if (!hasRequiredRules)
            {
                return Result.Success();
            }

            return Result.Failure(
                ResultType.ModelDataValidation,
                ResultError.InvalidInput(
                    "Rules",
                    $"Question '{question.Id}' of type '{question.Type}' must include 'rules' because this question type has required rules."
                )
            );
        }


        var requiredResult = ValidateRequiredRules(questionTypeDomain, question.Rules, requiredRules);
        if (requiredResult.IsFailure)
        {
            return requiredResult;
        }

        var typesResult = ValidateDataTypesOfRules(questionTypeDomain, question.Rules);
        if (typesResult.IsFailure)
        {
            return typesResult;
        }

        return Result.Success();
    }

    private Result ValidateRequiredRules(
        QuestionTypeDomain questionTypeDomain,
        Dictionary<string, ValidationRule> rulesMap,
        List<RuleDomain> requiredRules
    )
    {

        foreach (var rule in requiredRules)
        {
            if (!rulesMap.TryGetValue(rule.KeyName.Value, out var ruleValue))
            {
                return Result.Failure(
                    ResultType.ModelDataValidation,
                    ResultError.InvalidInput(
                        "Rule",
                        $"Question type '{questionTypeDomain.KeyName.Value}' requires rule '{rule.KeyName.Value}'."
                    )
                );
            }

            if (IsNullOrEmpty(ruleValue.Value))
            {
                return Result.Failure(
                    ResultType.ModelDataValidation,
                    ResultError.InvalidInput(
                        "Rule",
                        $"Rule '{rule.KeyName.Value}' on question type '{questionTypeDomain.KeyName.Value}' must have a non-empty value."
                    )
                );
            }
        }

        return Result.Success();
    }

    private Result ValidateDataTypesOfRules(
        QuestionTypeDomain questionTypeDomain,
        Dictionary<string, ValidationRule> rulesMap
    )
    {
        var rulesByName = questionTypeDomain.QuestionTypeRules
            .Select(x => x.Rule)
            .DistinctBy(r => r.KeyName.Value)
            .ToDictionary(r => r.KeyName.Value, StringComparer.OrdinalIgnoreCase);

        foreach (var (ruleName, ruleValue) in rulesMap)
        {
            if (!rulesByName.TryGetValue(ruleName, out var ruleDomain))
            {
                return Result.Failure(
                    ResultType.ModelDataValidation,
                    ResultError.InvalidInput(
                        "QuestionTypeRule",
                        $"Rule '{ruleName}' does not belong to question type '{questionTypeDomain.KeyName.Value}'."
                    )
                );
            }

            var dataTypeName = ruleDomain.DataType.Description.Value;

            var validateResult = SurveyDomainMethod.ValidateDataType(ruleName, dataTypeName, ruleValue.Value);
            if (validateResult.IsFailure)
            {
                return validateResult;
            }
        }

        return Result.Success();
    }

    private Dictionary<string, JsonElement> ToPropertyMap(JsonElement properties)
    {
        var map = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);

        if (properties.ValueKind != JsonValueKind.Object)
        {
            return map;
        }

        foreach (var prop in properties.EnumerateObject())
        {
            map[prop.Name] = prop.Value;
        }

        return map;
    }

    private Result ValidateRequiredProperties(
            QuestionTypeDomain questionTypeDomain,
            Dictionary<string, JsonElement> props
        )
    {
        List<AttributeDomain> requiredAttributes = questionTypeDomain.QuestionTypeAttributes
            .Where(x => x.IsRequired)
            .Select(x => x.Attribute)
            .DistinctBy(attr => attr.KeyName.Value)
            .ToList();

        foreach (var attr in requiredAttributes)
        {
            if (!props.TryGetValue(attr.KeyName.Value, out var value))
            {
                return Result.Failure(
                    ResultType.ModelDataValidation,
                    ResultError.InvalidInput(
                        "Attribute",
                        $"Question type '{questionTypeDomain.KeyName.Value}' requires attribute '{attr.KeyName.Value}'."
                    )
                );
            }
            if (IsNullOrEmpty(value))
            {
                return Result.Failure(
                    ResultType.ModelDataValidation,
                    ResultError.InvalidInput(
                        "Attribute",
                        $"Attribute '{attr.KeyName.Value}' on question type '{questionTypeDomain.KeyName.Value}' must have a non-empty value."
                    )
                );
            }

        }

        return Result.Success();
    }
    private static bool IsNullOrEmpty(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return true;

        }

        if (value.ValueKind == JsonValueKind.String)
        {
            return string.IsNullOrWhiteSpace(value.GetString());
        }

        return false;
    }

    private Result ValidateDataTypesOfProperties(
        QuestionTypeDomain questionTypeDomain,
        Dictionary<string, JsonElement> props
        )
    {
        var attributesByName = questionTypeDomain.QuestionTypeAttributes
            .Select(x => x.Attribute)
            .DistinctBy(a => a.KeyName.Value)
            .ToDictionary(a => a.KeyName.Value, StringComparer.OrdinalIgnoreCase);

        foreach (var (propertyName, value) in props)
        {
            if (!attributesByName.TryGetValue(propertyName, out var attribute))
            {
                return Result.Failure(
                    ResultType.ModelDataValidation,
                    ResultError.InvalidInput(
                        "QuestionTypeAttribute",
                        $"Attribute '{propertyName}' does not belong to question type '{questionTypeDomain.KeyName.Value}'."
                    )
                );
            }

            var dataTypeName = attribute.DataType.Description.Value;
            var validateResult = SurveyDomainMethod.ValidateDataType(propertyName, dataTypeName, value);

            if (validateResult.IsFailure)
            {
                return validateResult;
            }
        }

        return Result.Success();
    }
    private Result ValidateUniqueAttributeAcrossForm(
        IReadOnlyList<QuestionToValidate> questions,
        IReadOnlyList<QuestionTypeDomain> questionsType
    )
    {
        var uniqueAttributes = questionsType
            .SelectMany(qt => qt.QuestionTypeAttributes.Select(x => x.Attribute))
            .Where(a => a.MustBeUnique)
            .DistinctBy(a => a.KeyName.Value)
            .ToList();

        var questionProps = questions.Select(q => new
        {
            q.Id,
            q.Type,
            Props = ToPropertyMap(q.Properties)
        }).ToList();

        foreach (var attr in uniqueAttributes)
        {
            var seen = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

            foreach (var q in questionProps)
            {
                if (!q.Props.TryGetValue(attr.KeyName.Value, out var rawValue))
                {
                    continue;
                }

                if (!SurveyDomainMethod.TryConvertScalar(attr.KeyName.Value, rawValue, "Attribute", out var valueToStore, out var convertError))
                {
                    return convertError!;
                }
                if (valueToStore is null)
                {
                    continue;

                }

                valueToStore = valueToStore.Trim();
                if (string.IsNullOrWhiteSpace(valueToStore))
                {
                    continue;
                }

                if (seen.TryGetValue(valueToStore, out var firstQuestionId))
                {
                    var error = ResultError.InvalidInput(
                        "Attribute",
                        $"Attribute '{attr.KeyName.Value}' must be unique across all questions. " +
                        $"Duplicate value: '{valueToStore}'. First question id: '{firstQuestionId}', duplicate question id: '{q.Id}'."
                    );

                    return Result.Failure(ResultType.Conflict, error);
                }

                seen[valueToStore] = q.Id;
            }
        }

        return Result.Success();
    }


}
