using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;
internal sealed class ValidateQuestionDtoCommandHandler(
    IQuestionTypeRepository _questionTypeRepository
    ) : ICommandHandler<ValidateQuestionDtoCommand>
{
    public async Task<Result> Handle(ValidateQuestionDtoCommand request, CancellationToken cancellationToken)
    {
        var questions = request.Questions;

        var questionsTypeResult = await GetQuestionType(questions, cancellationToken);
        if (questionsTypeResult.IsFailure)
        {
            return Result.Failure(ResultType.NotFound, questionsTypeResult.Errors);
        }

        List<QuestionTypeDomain> questionsType = questionsTypeResult.Value;
        Result validatePropertiesResult = ValidateProperties(questions, questionsType);
        if (validatePropertiesResult.IsFailure)
        {
            return Result.Failure(ResultType.NotFound, validatePropertiesResult.Errors);
        }
        return Result.Success();
    }
    private Result ValidateProperties(List<QuestionDto> questionsDto, List<QuestionTypeDomain> listQuestionType)
    {
        foreach (var questionDto in questionsDto)
        {
            var questionType = listQuestionType.Find(x => x.KeyName.Value == questionDto.Type);
            if (questionType is null)
            {
                var errorQuestion = ResultError.InvalidInput(
                    "QuestionType",
                    $"The following question type were not found: {questionDto.Type}."
                );

                return Result.Failure(ResultType.NotFound, errorQuestion);
            }

            var properties = questionDto.Properties;
            var resultValidateRequiredProperties = ValidateRequieredProperties(questionType, properties);
            if (resultValidateRequiredProperties.IsFailure)
            {
                return Result.Failure(ResultType.ModelDataValidation, resultValidateRequiredProperties.Errors);
            }
            var validatePropertiesResult = ValidateDataTypeOfProperties(questionType, properties);
            if (validatePropertiesResult.IsFailure)
            {
                return Result.Failure(ResultType.ModelDataValidation, validatePropertiesResult.Errors);
            }
        }

        var resultUniqueAttributeAcrossForm = ValidateUniqueAttributeAcrossForm(questionsDto, listQuestionType);
        if (resultUniqueAttributeAcrossForm.IsFailure)
        {
            return Result.Failure(ResultType.ModelDataValidation, resultUniqueAttributeAcrossForm.Errors);
        }

        return Result.Success();
    }
    private Result ValidateRequieredProperties(QuestionTypeDomain questionTypeDomain, JsonElement properties)
    {
        List<AttributeDomain> requiredAttributes = questionTypeDomain.QuestionTypeAttributes
            .Where(x => x.IsRequired)
            .Select(x => x.Attribute)
            .DistinctBy(attr => attr.KeyName.Value)
            .ToList();

        foreach (var attribute in requiredAttributes)
        {
            var property = properties
                .EnumerateObject()
                .FirstOrDefault(prop =>
                    string.Equals(attribute.KeyName.Value, prop.Name, StringComparison.OrdinalIgnoreCase));

            var isUndefined = property.Value.ValueKind == JsonValueKind.Undefined;
            var isNull = property.Value.ValueKind == JsonValueKind.Null;
            var isEmptyValue = property.Value.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(property.Value.GetString());

            if (isUndefined || isNull || isEmptyValue)
            {
                var error = ResultError.InvalidInput(
                    "Attribute",
                    $"The question of type '{questionTypeDomain.KeyName.Value}' must include a non-empty value for the required attribute '{attribute.KeyName.Value}'."
                );
                return Result.Failure(ResultType.Conflict, error);
            }
        }

        return Result.Success();
    }
    private Result ValidateUniqueAttributeAcrossForm(List<QuestionDto> questionsDto, List<QuestionTypeDomain> listQuestionType)
    {
        List<AttributeDomain> attributesThatHasUniqueValue = listQuestionType
                                                .SelectMany(qt =>
                                                        qt.QuestionTypeAttributes.Select(x => x.Attribute)
                                                    )
                                                .Where(x => x.MustBeUnique)
                                                .DistinctBy(attr => attr.KeyName.Value)
                                                .ToList();


        foreach (var attributeMustHaveUniqueValue in attributesThatHasUniqueValue)
        {
            List<AttributeValueOccurrence> duplicateCandidates = questionsDto
                                                                    .SelectMany(x => x.Properties
                                                                                    .EnumerateObject()
                                                                                    .Where(prop =>
                                                                                        string.Equals(attributeMustHaveUniqueValue.KeyName.Value, prop.Name, StringComparison.OrdinalIgnoreCase)
                                                                                    )
                                                                                    .Select(prop => new AttributeValueOccurrence
                                                                                    {
                                                                                        IdQuestionType = x.Id,
                                                                                        QuestionType = x.Type,
                                                                                        AttributeName = attributeMustHaveUniqueValue.KeyName.Value,
                                                                                        Value = prop.Value
                                                                                    }).ToList()
                                                                        )
                                                                    .ToList();


            var duplicateGroups = duplicateCandidates
                .GroupBy(x => x.Value.ValueKind == JsonValueKind.String ? x.Value.GetString() : x.Value.ToString(), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateGroups.Any())
            {
                var error = ResultError.InvalidInput(
                    "Attribute",
                    $"The attribute '{attributeMustHaveUniqueValue.KeyName.Value}' must be unique across all questions."
                );
                return Result.Failure(ResultType.Conflict, error);
            }

        }

        return Result.Success();
    }
    private Result ValidateDataTypeOfProperties(QuestionTypeDomain questionTypeDomain, JsonElement properties)
    {
        List<AttributeDomain> attributesOfQuestionType = questionTypeDomain.QuestionTypeAttributes.Select(x => x.Attribute).ToList();
        foreach (var property in properties.EnumerateObject())
        {
            string propertyName = property.Name;
            AttributeDomain? attribute = attributesOfQuestionType
                                                .Find(x =>
                                                        string.Equals(x.KeyName.Value, propertyName, StringComparison.OrdinalIgnoreCase)
                                                    );

            if (attribute is null)
            {
                var errorQuestion = ResultError.InvalidInput(
                    "QuestionTypeAttribute",
                    $"The following question type attribute dosen't belong to the questionType {questionTypeDomain.KeyName}: {propertyName} ."
                );
                return Result.Failure(ResultType.NotFound, errorQuestion);
            }
            JsonElement value = property.Value;
            var dataType = attribute.DataType.Description.Value.ToLowerInvariant();
            var validationResult = SurveyCommonMethods.ValidateDataType(propertyName, dataType, value);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }
        }
        return Result.Success();
    }
    private async Task<ResultT<List<QuestionTypeDomain>>> GetQuestionType(List<QuestionDto> questionsDto, CancellationToken cancellationToken)
    {
        var questionTypeRequest = questionsDto.Select(x => x.Type).DistinctBy(x => x).ToList();
        var questionsType = await _questionTypeRepository
                                    .GetByTypeKeysAsync(questionTypeRequest, true, cancellationToken);

        var foundKeys = questionsType.Select(q => q.KeyName.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var notFoundKeys = questionTypeRequest
            .Where(key => !foundKeys.Contains(key))
            .ToList();

        if (notFoundKeys.Any())
        {
            var notFoundList = string.Join(", ", notFoundKeys);
            var errorQuestion = ResultError.InvalidInput(
                "QuestionType",
                $"The following question type(s) were not found: {notFoundList}."
            );

            return ResultT<List<QuestionTypeDomain>>.FailureT(ResultType.NotFound, errorQuestion);
        }
        return questionsType;
    }

}
