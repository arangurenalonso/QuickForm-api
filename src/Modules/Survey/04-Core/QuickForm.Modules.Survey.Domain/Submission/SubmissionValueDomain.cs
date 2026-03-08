using System.Globalization;
using System.Text.Json;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;

namespace QuickForm.Modules.Survey.Domain;

public sealed class SubmissionValueDomain : BaseDomainEntity<SubmissionValueId>
{
    public SubmissionId IdSubmission { get; private set; }
    public QuestionId IdQuestion { get; private set; }
    public string ValueRaw { get; private set; } = default!;
    public string DisplayValue { get; private set; } = string.Empty;
    public decimal? ValueDecimal { get; private set; }
    public long? ValueInteger { get; private set; }
    public DateTime? ValueDateTime { get; private set; }
    public bool? ValueBoolean { get; private set; }

    public SubmissionDomain Submission { get; private set; } = default!;
    public QuestionDomain Question { get; private set; } = default!;

    private SubmissionValueDomain() { }

    private SubmissionValueDomain(
        SubmissionValueId id,
        SubmissionId idSubmission,
        QuestionId idQuestion,
        string valueRaw,
        string displayValue,
        decimal? valueDecimal,
        long? valueInteger,
        DateTime? valueDateTime,
        bool? valueBoolean
    ) : base(id)
    {
        IdSubmission = idSubmission;
        IdQuestion = idQuestion;
        ValueRaw = valueRaw;
        DisplayValue = displayValue;
        ValueDecimal = valueDecimal;
        ValueInteger = valueInteger;
        ValueDateTime = valueDateTime;
        ValueBoolean = valueBoolean;
    }


    public static ResultT<SubmissionValueDomain> Create(
        SubmissionId idSubmission,
        QuestionForSubmission question,
        string name,
        JsonElement value
    )
    {
        var errors = new List<ResultError>();

        var attrValidation = ValidateAttributes(value, question, name);
        if (attrValidation.IsFailure)
        {
            errors.AddRange(attrValidation.Errors.ToList());
        }

        foreach (var rule in question.Rules)
        {
            var validationResult = ValidateRules(value, rule, question.QuestionId, name);
            if (validationResult.IsFailure)
            {
                errors.AddRange(validationResult.Errors.ToList());
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        var storedResult = BuildStoredValues(value, question.QuestionTypeId);
        if (storedResult.IsFailure)
        {
            return storedResult.Errors;
        }
        var stored = storedResult.Value;

        return new SubmissionValueDomain(
           SubmissionValueId.Create(),
           idSubmission,
           question.QuestionId,
           stored.RawValue,
           stored.DisplayValue,
           stored.ValueDecimal,
           stored.ValueInteger,
           stored.ValueDateTime,
           stored.ValueBoolean
       );
    }
    private static Result ValidateAttributes(JsonElement value, QuestionForSubmission question, string name)
    {
        // AllowNegative
        var allowNegativeAttr = question.Attributes.FirstOrDefault(a => a.AttributeId == AttributeType.AllowNegative.GetId());
        if (allowNegativeAttr.Value is not null)
        {
            if (!bool.TryParse(allowNegativeAttr.Value, out var allowNegative))
            {
                return ResultError.InvalidInput(
                    "FormDefinition",
                    $"Field '{name}' has AllowNegative attribute with invalid boolean value '{allowNegativeAttr.Value}'."
                );
            }

            if (!allowNegative && CommonJsonElementMethods.TryGetDecimal(value, out var dec) && dec < 0)
            {
                return ResultError.InvalidInput(name, "Negative values are not allowed.");
            }
        }

        // DecimalScale
        var decimalScaleAttr = question.Attributes.FirstOrDefault(a => a.AttributeId == AttributeType.DecimalScale.GetId());
        if (decimalScaleAttr.Value is not null)
        {
            if (!int.TryParse(decimalScaleAttr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var scale) || scale < 0)
            {
                return ResultError.InvalidInput(
                    "FormDefinition",
                    $"Field '{name}' has DecimalScale attribute with invalid integer value '{decimalScaleAttr.Value}'."
                );
            }

            if (CommonJsonElementMethods.TryGetDecimal(value, out var dec))
            {
                var decimals = SurveyDomainMethod.CountDecimals(dec);
                if (decimals > scale)
                {
                    return ResultError.InvalidInput(name, $"Maximum allowed decimal places is {scale}.");
                }
            }
        }

        return Result.Success();
    }

    private static Result ValidateRules(
        JsonElement value,
        (Guid RuleId, string? Value, string Message, string? Placeholder) rule,
        QuestionId questionId,
        string name
    )
    {

        switch (rule.RuleId)
        {
            case var r when r == RuleType.Required.GetId():
                {
                    if (string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!bool.TryParse(rule.Value, out var required))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has Required rule with invalid boolean value '{rule.Value}'."
                        );
                    }

                    if (required && CommonJsonElementMethods.IsEmpty(value))
                    {
                        var msg = SurveyDomainMethod.GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, "");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.MaxLength.GetId():
                {
                    if (CommonJsonElementMethods.IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!int.TryParse(rule.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var maxLen))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has MaxLength rule with invalid int value '{rule.Value}'."
                        );
                    }

                    if (value.ValueKind != JsonValueKind.String)
                    {
                        return ResultError.InvalidInput(name, "The value must be a string.");
                    }

                    var s = value.GetString() ?? string.Empty;
                    if (s.Length > maxLen)
                    {
                        var msg = SurveyDomainMethod.GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{maxLen}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.MinLength.GetId():
                {
                    if (CommonJsonElementMethods.IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!int.TryParse(rule.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var minLen))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has MinLength rule with invalid int value '{rule.Value}'."
                        );
                    }

                    if (value.ValueKind != JsonValueKind.String)
                    {
                        return ResultError.InvalidInput(name, "The value must be a string.");
                    }

                    var s = value.GetString() ?? string.Empty;
                    if (s.Length < minLen)
                    {
                        var msg = SurveyDomainMethod.GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{minLen}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.Min.GetId():
                {
                    if (CommonJsonElementMethods.IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }

                    if (!decimal.TryParse(rule.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var min))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has Min rule with invalid decimal value '{rule.Value}'."
                        );
                    }

                    if (!CommonJsonElementMethods.TryGetDecimal(value, out var dec))
                    {
                        return ResultError.InvalidInput(name, "The value must be a valid number.");
                    }

                    if (dec < min)
                    {
                        var msg = SurveyDomainMethod.GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{min}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            case var r when r == RuleType.Max.GetId():
                {
                    if (CommonJsonElementMethods.IsNull(value) || string.IsNullOrWhiteSpace(rule.Value))
                    {
                        return Result.Success();
                    }


                    if (!decimal.TryParse(rule.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var max))
                    {
                        return ResultError.InvalidInput(
                            "FormDefinition",
                            $"Question '{questionId.Value}' has Max rule with invalid decimal value '{rule.Value}'."
                        );
                    }

                    if (!CommonJsonElementMethods.TryGetDecimal(value, out var dec))
                    {
                        return ResultError.InvalidInput(name, "The value must be a valid number.");
                    }

                    if (dec > max)
                    {
                        var msg = SurveyDomainMethod.GetMessageFromTemplateAndPlaceholder(rule.Message, rule.Placeholder, $"{max}");
                        return ResultError.InvalidInput(name, msg);
                    }

                    return Result.Success();
                }

            default:
                return ResultError.InvalidInput(
                    "FormDefinition",
                    $"Validation for rule '{rule.RuleId}' is not implemented."
                );
        }
    }

    private static ResultT<(
        string RawValue,
        string DisplayValue,
        decimal? ValueDecimal,
        long? ValueInteger,
        DateTime? ValueDateTime,
        bool? ValueBoolean
    )> BuildStoredValues(JsonElement jsonElement, Guid idQuestionType)
    {
        var rawValue = jsonElement.GetRawText();
        var displayValue = string.Empty;

        decimal? valueDecimal = null;
        long? valueInteger = null;
        DateTime? valueDateTime = null;
        bool? valueBoolean = null;

        if (jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return (rawValue, displayValue, valueDecimal, valueInteger, valueDateTime, valueBoolean);
        }

        var questionType = EnumExtensions.FromId<QuestionTypeType>(idQuestionType);
        if (questionType is null)
        {
            throw new ArgumentException(
                $"Unknown question type id '{idQuestionType}'.",
                nameof(idQuestionType));
        }

        switch (questionType)
        {
            case QuestionTypeType.InputTypeText:
                {
                    displayValue = jsonElement.GetString()?.Trim() ?? string.Empty;
                    break;
                }

            case QuestionTypeType.InputTypeInteger:
                {
                    if (CommonJsonElementMethods.TryGetInt64(jsonElement, out var intValue))
                    {
                        valueInteger = intValue;
                        displayValue = intValue.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return ResultError.InvalidInput(
                                "Value",
                                $"The value '{rawValue}' is not a valid integer.");
                    }

                    break;
                }

            case QuestionTypeType.InputTypeDecimal:
                {
                    if (CommonJsonElementMethods.TryGetDecimal(jsonElement, out var decimalValue))
                    {
                        valueDecimal = decimalValue;
                        displayValue = SurveyDomainMethod.FormatDecimalDisplayPreservingScale(rawValue);
                    }
                    else
                    {
                        return ResultError.InvalidInput(
                                "Value",
                                $"The value '{rawValue}' is not a valid decimal number.");
                    }

                    break;
                }

            case QuestionTypeType.InputTypeBoolean:
                {
                    if (CommonJsonElementMethods.TryGetBoolean(jsonElement, out var boolValue))
                    {
                        valueBoolean = boolValue;
                        displayValue = boolValue ? "true" : "false";
                    }
                    else
                    {
                        return ResultError.InvalidInput(
                                "Value",
                                $"The value '{rawValue}' is not a valid boolean.");
                    }

                    break;
                }

            case QuestionTypeType.InputTypeDatetime:
                {
                    if (CommonJsonElementMethods.TryGetDateTime(jsonElement, out var dateTimeValue))
                    {
                        valueDateTime = dateTimeValue;
                        displayValue = valueDateTime.Value.ToString("O", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return ResultError.InvalidInput(
                                "Value",
                                $"The value '{rawValue}' is not a valid datetime.");
                    }

                    break;
                }

            case QuestionTypeType.InputTypeDate:
                {
                    if (CommonJsonElementMethods.TryGetDateOnlyAsDateTime(jsonElement, out var dateValue))
                    {
                        valueDateTime = DateTime.SpecifyKind(dateValue, DateTimeKind.Utc);
                        displayValue = dateValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return ResultError.InvalidInput(
                                "Value",
                                $"The value '{rawValue}' is not a valid date in 'yyyy-MM-dd' format.");
                    }

                    break;
                }

            case QuestionTypeType.InputTypeTime:
                {
                    if (CommonJsonElementMethods.TryGetTimeOnly(jsonElement, out var timeValue))
                    {
                        displayValue = timeValue.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return ResultError.InvalidInput(
                                "Value",
                                $"The value '{rawValue}' is not a valid time in 'HH:mm:ss' format.");
                    }

                    break;
                }

            default:
                {
                    throw new ArgumentException(
                        $"Unsupported question type '{questionType}'.",
                        nameof(idQuestionType));
                }
        }

        return (rawValue, displayValue, valueDecimal, valueInteger, valueDateTime, valueBoolean);
    }



}
