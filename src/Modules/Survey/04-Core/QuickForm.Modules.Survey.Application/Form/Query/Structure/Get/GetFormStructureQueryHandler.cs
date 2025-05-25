using System.Text.Json;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using static System.Collections.Specialized.BitVector32;

namespace QuickForm.Modules.Survey.Application;
internal sealed class GetFormStructureQueryHandler(
    IQuestionTypeRepository _questionTypeRepository,
    IFormRepository _formRepository, 
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
        List<FormSectionDomain> formSections = await _formRepository.GetStructureFormAsync(request.IdForm, true, cancellationToken);

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
    List<QuestionTypeDomain> questionsType)
    {
        var formStructureSectionsReponse = new List<FormStructureSectionReponse>();

        var sectionsOrdered = formSections.OrderBy(x => x.SortOrder);
        foreach (var section in sectionsOrdered)
        {
            var questionsResponse = new List<FormStructureQuestionReponse>();
            var questionsDomainOrdered = section.Questions.OrderBy(x => x.SortOrder);
            foreach (var question in questionsDomainOrdered)
            {
                var questionType = questionsType.Find(x=>x.Id == question.IdQuestionType);
                if (questionType is null)
                {
                    var errorQuestion = ResultError.InvalidInput(
                        "QuestionType",
                        $"The following question type were not found: {question.Id}."
                    );

                    return ResultT<List<FormStructureSectionReponse>>.FailureT(ResultType.NotFound, errorQuestion);
                }

                var propertyResult = GetProperty(question.QuestionAttributeValue.ToList(), questionType);
                if (propertyResult.IsFailure)
                {
                    return ResultT<List<FormStructureSectionReponse>>.FailureT(ResultType.NotFound, propertyResult.Errors);
                }

                var questionResponse = new FormStructureQuestionReponse()
                {
                    Id = question.Id.Value,
                    Type = questionType.KeyName.Value,
                    Properties = propertyResult.Value
                };
                questionsResponse.Add(questionResponse);

            }

            var sectionResponse = new FormStructureSectionReponse()
            {
                Id = section.Id.Value,
                Title = section.Title.Value,
                Description = section.Description.Value,
                Questions = questionsResponse,                

            };
            formStructureSectionsReponse.Add(sectionResponse);
        }

        return formStructureSectionsReponse;
    }

    private ResultT<JsonElement> GetProperty(
        List<QuestionAttributeValueDomain> attributes,
        QuestionTypeDomain questionType)
    {
        var obj = new Dictionary<string, object>();

        foreach (var attribute in attributes)
        {
            var questionTypeAttribute = questionType.QuestionTypeAttributes.FirstOrDefault(x => x.Id == attribute.IdQuestionTypeAttribute);
            if (questionTypeAttribute is null)
            {
                var errorQuestionTypeAttribute = ResultError.InvalidInput(
                    "QuestionTypeAttribute",
                    $"The following question type attribute were not found: {attribute.IdQuestionTypeAttribute}."
                );

                return ResultT<JsonElement>.FailureT(ResultType.NotFound, errorQuestionTypeAttribute);
            }
            var attributeName = questionTypeAttribute.Attribute.KeyName.Value;
            if (obj.ContainsKey(attributeName))
            {
                var error = ResultError.InvalidInput(
                    "Attribute",
                    $"Duplicate attribute name found: '{attributeName}'."
                );
                return ResultT<JsonElement>.FailureT(ResultType.Conflict, error);
            }
            var valueToConvert = attribute.Value;
            var dataType = questionTypeAttribute.Attribute.DataType.Description.Value.ToLowerInvariant();
            var valueConvertedResult = SurveyCommonMethods.ConvertValue(valueToConvert, dataType);
            if (valueConvertedResult.IsFailure)
            {
                return ResultT<JsonElement>.FailureT(ResultType.NotFound, valueConvertedResult.Errors);
            }
            obj[attributeName] = valueConvertedResult.Value;

        }


        var json = JsonSerializer.Serialize(obj);
        using var document = JsonDocument.Parse(json);
        var jsonElement = document.RootElement.Clone();
        return jsonElement;
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
