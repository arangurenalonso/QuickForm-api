using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;


internal sealed class FormQuestionRegisterCommandHandler(
    IUnitOfWork _unitOfWork,
    IFormRepository _formRepository,
    IQuestionRepository _questionRepository,
    IQuestionTypeRepository _questionTypeRepository,
    ICurrentUserService _currentUserService,
    ICustomerRepository _customerRepository
    ) : ICommandHandler<FormQuestionRegisterCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(FormQuestionRegisterCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = _currentUserService.UserId;
        if (userIdResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, userIdResult.Errors);
        }
        var formResult=await GetForm(request.IdForm, userIdResult.Value,cancellationToken);
        if (formResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, formResult.Errors);
        }
        FormDomain formDomain = formResult.Value;
        var questionsTypeResult = await GetQuestionType(request.Questions, cancellationToken);
        if (questionsTypeResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, questionsTypeResult.Errors);
        }

        var questionsType = questionsTypeResult.Value;
        var validatePropertiesResult = ValidateProperties(request.Questions, questionsType);
        if (validatePropertiesResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, validatePropertiesResult.Errors);
        }

        var resultStoreDatabase = await StoreDatabase(formDomain, request.Questions, questionsType,cancellationToken);
        if (resultStoreDatabase.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultStoreDatabase.ResultType, resultStoreDatabase.Errors);
        }

        return ResultTResponse<Guid>.Success(Guid.NewGuid(), $".");
    }
    private async  Task<Result> StoreDatabase(
        FormDomain formDomain,
        List<QuestionDto> questionsDto, 
        List<QuestionTypeDomain> listQuestionType,
        CancellationToken cancellationToken)
    {
        List<QuestionDomain> questions = await GetQuestion(formDomain.Id.Value, cancellationToken);
        var incomingIds = questionsDto.Select(q =>new QuestionId(q.Id)).ToHashSet();

        List<QuestionDomain> questionsToDesactivate = questions.Where(x => !incomingIds.Contains(x.Id)).ToList();

        foreach (var questionToDesactivate in questionsToDesactivate)
        {
            questionToDesactivate.Deactivate();
            _questionRepository.Update(questionToDesactivate);
        }

        int order = 1;


        foreach (var questionDto in questionsDto)
        {
            QuestionTypeDomain questionType = listQuestionType.Find(x => x.KeyName.Value == questionDto.Type);
            if (questionType is null)
            {
                var errorQuestion = ResultError.InvalidInput(
                    "QuestionType",
                    $"The following question type were not found: {questionDto.Type}."
                );

                return Result.Failure(ResultType.NotFound, errorQuestion);
            }
            var questionId=new QuestionId(questionDto.Id);
            var questionDtoPropertie = questionDto.Properties;

            var questionDomainExisted = questions.Find(x => x.Id == questionId);
            if (questionDomainExisted is null)
            {
                var questionCreatedResult = QuestionDomain.Create(questionId,formDomain.Id, questionType.Id, order);
                if (questionCreatedResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, questionCreatedResult.Errors);
                }
                var questionCreated= questionCreatedResult.Value;
                _questionRepository.Insert(questionCreated);



                var resultStoreQuestionAttributeValue = StoreDatabaseQuestionAttributeValue(questionCreated.Id, questionDtoPropertie, questionType);
                if (resultStoreQuestionAttributeValue.IsFailure)
                {
                    return Result.Failure(resultStoreQuestionAttributeValue.ResultType, resultStoreQuestionAttributeValue.Errors);
                }

            }
            else
            {
                questionDomainExisted.Update(order);
                _questionRepository.Update(questionDomainExisted);

                var resultStoreQuestionAttributeValue = StoreDatabaseQuestionAttributeValue(
                                                                    questionDomainExisted.Id,
                                                                    questionDtoPropertie,
                                                                    questionType,
                                                                    questionDomainExisted.QuestionAttributeValue.ToList()
                                                                );
                if (resultStoreQuestionAttributeValue.IsFailure)
                {
                    return Result.Failure(resultStoreQuestionAttributeValue.ResultType, resultStoreQuestionAttributeValue.Errors);
                }

            }
            order++;
        }




        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);

        if (resultTransaction.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultTransaction.ResultType, resultTransaction.Errors);
        }

        return Result.Success();
    }
    private Result StoreDatabaseQuestionAttributeValue(
        QuestionId idQuestion,
        JsonElement properties, 
        QuestionTypeDomain questionType,
        List<QuestionAttributeValueDomain>? listQuestionAttributeValue = null
        )
    {

        var questionTypesAttribute = questionType.QuestionTypeAttributes.ToList();


        var incomingKeys = properties.EnumerateObject()
                             .Select(p => p.Name)
                             .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (listQuestionAttributeValue is not null)
        {
            var toDisable = listQuestionAttributeValue
                                .Where(x => !questionTypesAttribute
                                                .Any(qta =>
                                                    string.Equals(qta.Attribute.KeyName.Value, x.Value, StringComparison.OrdinalIgnoreCase)
                                                    && incomingKeys.Contains(qta.Attribute.KeyName.Value))
                                );

            foreach (var item in toDisable)
            {
                item.Deactivate();
                _questionRepository.Update(item);
            }
        }
        foreach (var property in properties.EnumerateObject())
        {
            string propertyName = property.Name;
            QuestionTypeAttributeDomain? questionTypeAttribute = questionTypesAttribute
                                                                    .Find(x =>
                                                                            string.Equals(x.Attribute.KeyName.Value, propertyName, StringComparison.OrdinalIgnoreCase)
                                                                        );

            if (questionTypeAttribute is null)
            {
                var errorQuestion = ResultError.InvalidInput(
                                        "QuestionTypeAttribute",
                                        $"The attribute '{propertyName}' is not defined for the question type '{questionType.KeyName}'. Please verify the configuration of the question type."
                                    );

                return Result.Failure(ResultType.NotFound, errorQuestion);
            }
            var questionAttributeValueExisted = listQuestionAttributeValue?.Find(x => x.IdQuestionTypeAttribute == questionTypeAttribute.Id);

            JsonElement value = property.Value;
            string? valueToStore = value.ValueKind == JsonValueKind.Null ? null : value.ToString();

            if (questionAttributeValueExisted is null)
            {

                var resultQuestionAttributeValueCreated = QuestionAttributeValueDomain.Create(
                                                                        idQuestion,
                                                                        questionTypeAttribute.Id,
                                                                        valueToStore
                                                                    );
                if (resultQuestionAttributeValueCreated.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, resultQuestionAttributeValueCreated.Errors);
                }

                var questionAttributeValueCreated = resultQuestionAttributeValueCreated.Value;
                _questionRepository.Insert( questionAttributeValueCreated );
            }
            else
            {
                questionAttributeValueExisted.Update(valueToStore);
                _questionRepository.Update(questionAttributeValueExisted);
            }

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
            var resultValidateRequiredProperties= ValidateRequieredProperties(questionType, properties);
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

        var resultUniqueAttributeValidation = ValidateUniqueAttributePerQuestion(questionsDto, listQuestionType);
        if (resultUniqueAttributeValidation.IsFailure)
        {
            return Result.Failure(ResultType.ModelDataValidation, resultUniqueAttributeValidation.Errors);
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
    private Result ValidateUniqueAttributePerQuestion(List<QuestionDto> questionsDto, List<QuestionTypeDomain> listQuestionType)
    {
        List<AttributeDomain> attributesThatHasUniqueValue = listQuestionType
                                                .SelectMany(qt => 
                                                        qt.QuestionTypeAttributes.Select(x => x.Attribute)
                                                    )
                                                .Where(x=>x.MustBeUnique)
                                                .DistinctBy(attr => attr.KeyName.Value)
                                                .ToList();


        foreach (var attributeMustHaveUniqueValue in attributesThatHasUniqueValue)
        {
            List<AttributeValueOccurrence> duplicateCandidates = questionsDto
                                                                    .SelectMany(x => x.Properties
                                                                                    .EnumerateObject()
                                                                                    .Where(prop =>
                                                                                        string.Equals(attributeMustHaveUniqueValue.KeyName.Value,prop.Name, StringComparison.OrdinalIgnoreCase)
                                                                                    )
                                                                                    .Select(prop => new AttributeValueOccurrence
                                                                                    {
                                                                                        IdQuestionType=x.Id,
                                                                                        QuestionType=x.Type,
                                                                                        AttributeName = attributeMustHaveUniqueValue.KeyName.Value,
                                                                                        Value=prop.Value
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
    private async Task<ResultT<List<QuestionTypeDomain>>> GetQuestionType(List<QuestionDto> questionsDto,CancellationToken cancellationToken)
    {
        var questionTypeRequest= questionsDto.Select(x=>x.Type).DistinctBy(x => x).ToList();
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
    private async Task<ResultT<FormDomain>> GetForm(Guid idForm, Guid idCustomer,CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetAsync(idCustomer, cancellationToken);
        if (customer is null)
        {
            var errorCustomer = ResultError.InvalidInput(
           "Customer",
           $"No customer was found with the ID '{idCustomer}'. Please verify the provided customer identifier."
            );
            return ResultT<FormDomain>.FailureT(ResultType.NotFound, errorCustomer);
        }
        var form  = await _formRepository.GetAsync(idForm, cancellationToken);
        if (form is null)
        {
            var errorForm = ResultError.InvalidInput(
            "Form",
            $"No form was found with the ID '{idForm}'. Please ensure the form exists and the ID is correct."
            );
            return ResultT<FormDomain>.FailureT(ResultType.NotFound, errorForm);
        }
        if (form.IdCustomer != customer.Id)
        {
            var errorForm = ResultError.InvalidInput(
            "Form",
            $"The form with ID '{idForm}' does not belong to the customer with ID '{customer.FullName}'. Access is not allowed."
            );
            return ResultT<FormDomain>.FailureT(ResultType.Forbidden, errorForm);
        }
        return form;

    }
    private async Task<List<QuestionDomain>> GetQuestion(Guid idForm, CancellationToken cancellationToken)
    {
        var questions = await _formRepository.GetQuestionAsync(idForm, cancellationToken);
        return questions.ToList();
    }


}
