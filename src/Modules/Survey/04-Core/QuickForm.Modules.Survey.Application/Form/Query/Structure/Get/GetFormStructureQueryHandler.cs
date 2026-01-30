using System.Text.Json;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application.Forms.Queries;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class GetFormStructureQueryHandler(
    IQuestionTypeRepository _questionTypeRepository,
    IFormQueries _formQuery,
    IUnitOfWork _unitOfWork)
    : IQueryHandler<GetFormStructureQuery, List<FormStructureSectionReponse>>
{
    public async Task<ResultT<List<FormStructureSectionReponse>>> Handle(GetFormStructureQuery request, CancellationToken cancellationToken)
    {
        var formId = new FormId(request.IdForm);
        var existForm=await _unitOfWork.Repository<FormDomain,FormId>().ExistEntity(formId);
        if (!existForm)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<List<FormStructureSectionReponse>>.FailureT(ResultType.NotFound, error);
        }
        List<FormSectionDomain> formSections = await _formQuery.GetStructureFormAsync(request.IdForm, true, cancellationToken);

        List<QuestionDomain> questions = formSections.SelectMany(x=>x.Questions).ToList();
        var questionsTypeResult = await GetQuestionType(questions, cancellationToken);
        if (questionsTypeResult.IsFailure)
        {
            return ResultT<List<FormStructureSectionReponse>>.FailureT(ResultType.NotFound, questionsTypeResult.Errors);
        }
        List<QuestionTypeDomain> questionsType = questionsTypeResult.Value;

       
        var result = MapSections(formSections, questionsType);
        return result;

    }
    private ResultT<List<FormStructureSectionReponse>> MapSections(
            List<FormSectionDomain> formSections,
            List<QuestionTypeDomain> questionsType
        )
    {
        var qtById = questionsType.ToDictionary(x => x.Id, x => x);

        var sectionsResponse = new List<FormStructureSectionReponse>();

        foreach (var section in formSections.OrderBy(s => s.SortOrder))
        {
            var questionsResponse = new List<FormStructureQuestionReponse>();

            foreach (var question in section.Questions.OrderBy(q => q.SortOrder))
            {
                if (!qtById.TryGetValue(question.IdQuestionType, out var questionType))
                {
                    return ResultT<List<FormStructureSectionReponse>>.FailureT(
                        ResultType.NotFound,
                        ResultError.InvalidInput(
                            "QuestionType",
                            $"Question type with id '{question.IdQuestionType.Value}' not found."
                        )
                    );
                }

                var propsResult = GetProperty(question.QuestionAttributeValue.ToList(), questionType);
                if (propsResult.IsFailure)
                {
                    return ResultT<List<FormStructureSectionReponse>>.FailureT(
                        ResultType.NotFound,
                        propsResult.Errors
                    );
                }

                var rulesResult = GetRules(question.QuestionRuleValue.ToList(), questionType);
                if (rulesResult.IsFailure)
                {
                    return ResultT<List<FormStructureSectionReponse>>.FailureT(
                        ResultType.NotFound,
                        rulesResult.Errors
                    );
                }

                questionsResponse.Add(new FormStructureQuestionReponse
                {
                    Id = question.Id.Value,
                    Type = questionType.KeyName.Value,
                    Properties = propsResult.Value,
                    Rules = rulesResult.Value
                });
            }

            sectionsResponse.Add(new FormStructureSectionReponse
            {
                Id = section.Id.Value,
                Title = section.Title.Value,
                Description = section.Description.Value,
                Questions = questionsResponse
            });
        }

        return sectionsResponse;
    }


    private ResultT<JsonElement> GetProperty(
    List<QuestionAttributeValueDomain> attributes,
    QuestionTypeDomain questionType)
    {
        var obj = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        var typeAttrById = questionType.QuestionTypeAttributes
            .ToDictionary(x => x.Id, x => x);

        foreach (var attribute in attributes)
        {
            if (!typeAttrById.TryGetValue(attribute.IdQuestionTypeAttribute, out var questionTypeAttribute))
            {
                var err = ResultError.InvalidInput(
                    "QuestionTypeAttribute",
                    $"The following question type attribute were not found: {attribute.IdQuestionTypeAttribute}.");
                return ResultT<JsonElement>.FailureT(ResultType.NotFound, err);
            }

            var attributeName = questionTypeAttribute.Attribute.KeyName.Value;

            if (obj.ContainsKey(attributeName))
            {
                var err = ResultError.InvalidInput(
                    "Attribute",
                    $"Duplicate attribute name found: '{attributeName}'.");
                return ResultT<JsonElement>.FailureT(ResultType.Conflict, err);
            }

            var dataType = questionTypeAttribute.Attribute.DataType.Description.Value.ToLowerInvariant();
            var valueConvertedResult = SurveyCommonMethods.ConvertValue(attribute.Value, dataType);

            if (valueConvertedResult.IsFailure)
            {
                return ResultT<JsonElement>.FailureT(ResultType.NotFound, valueConvertedResult.Errors);
            }

            obj[attributeName] = valueConvertedResult.Value;
        }

        var element = JsonSerializer.SerializeToElement(obj);
        return element;
    }
    private ResultT<Dictionary<string, RuleQuestionResponseDto>> GetRules(
    List<QuestionRuleValueDomain> rules,
    QuestionTypeDomain questionType)
    {
        var result = new Dictionary<string, RuleQuestionResponseDto>(StringComparer.OrdinalIgnoreCase);

        var typeRuleById = questionType.QuestionTypeRules
            .ToDictionary(x => x.Id, x => x);

        foreach (var ruleValue in rules.Where(r => !r.IsDeleted))
        {
            if (!typeRuleById.TryGetValue(ruleValue.IdQuestionTypeRule, out var typeRule))
            {
                var err = ResultError.InvalidInput(
                    "QuestionTypeRule",
                    $"The following question type rule were not found: {ruleValue.IdQuestionTypeRule}.");
                return ResultT<Dictionary<string, RuleQuestionResponseDto>>.FailureT(ResultType.NotFound, err);
            }

            var ruleName = typeRule.Rule.KeyName.Value;

            if (result.ContainsKey(ruleName))
            {
                var err = ResultError.InvalidInput(
                    "Rule",
                    $"Duplicate rule name found: '{ruleName}'.");
                return ResultT<Dictionary<string, RuleQuestionResponseDto>>.FailureT(ResultType.Conflict, err);
            }

            // Convert string stored value -> proper JSON based on rule datatype
            var dataType = typeRule.Rule.DataType.Description.Value.ToLowerInvariant();

            var valueConvertedResult = SurveyCommonMethods.ConvertValue(ruleValue.Value, dataType);
            if (valueConvertedResult.IsFailure)
            {
                return ResultT<Dictionary<string, RuleQuestionResponseDto>>.FailureT(ResultType.NotFound, valueConvertedResult.Errors);
            }

            // Convert to JsonElement to match API contract
            var jsonValue = JsonSerializer.SerializeToElement(valueConvertedResult.Value);

            result[ruleName] = new RuleQuestionResponseDto
            {
                Value = jsonValue,
                Message = ruleValue.Message // ajusta nombre propiedad si es distinto
            };
        }

        return result;
    }

    private async Task<ResultT<List<QuestionTypeDomain>>> GetQuestionType(List<QuestionDomain> questionsDomain, CancellationToken cancellationToken)
    {
        var idsQuestionsType = questionsDomain.Select(x => x.IdQuestionType).DistinctBy(x => x).ToList();
        var questionsType = await _questionTypeRepository
                                    .GetByIdsAsync(idsQuestionsType, true, cancellationToken);

        var foundIds = questionsType.Select(x => x.Id).ToHashSet();
        var notFoundIds = idsQuestionsType.Where(id => !foundIds.Contains(id)).ToList();

        if (notFoundIds.Any())
        {
            var notFoundList = string.Join(", ", notFoundIds);
            var errorQuestion = ResultError.InvalidInput(
                "QuestionType",
                $"The following question type(s) were not found: {notFoundList}."
            );

            return ResultT<List<QuestionTypeDomain>>.FailureT(ResultType.NotFound, errorQuestion);
        }
        return questionsType;
    }

}
