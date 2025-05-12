using MediatR;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;
internal sealed class SaveFormStructureCommandHandler(
    IUnitOfWork _unitOfWork,
    IFormRepository _formRepository,
    IQuestionTypeRepository _questionTypeRepository,
    ICurrentUserService _currentUserService,
    ICustomerRepository _customerRepository,
    IMediator _mediator
    ) : ICommandHandler<SaveFormStructureCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(SaveFormStructureCommand request, CancellationToken cancellationToken)
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

        var questions = request.Sections.SelectMany(x => x.Questions).ToList();

        var validatePropertiesResult = await _mediator.Send(new ValidateQuestionDtoCommand(questions),cancellationToken);
        if (validatePropertiesResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, validatePropertiesResult.Errors);
        }

        var questionsTypeResult = await GetQuestionType(questions, cancellationToken);
        if (questionsTypeResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, questionsTypeResult.Errors);
        }

        List<QuestionTypeDomain> questionsType = questionsTypeResult.Value;

        var resultStoreDatabase = await StoreDatabase(formDomain, request.Sections, questionsType,cancellationToken);
        if (resultStoreDatabase.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultStoreDatabase.ResultType, resultStoreDatabase.Errors);
        }

        return ResultTResponse<Guid>.Success(Guid.NewGuid(), $".");
    }
    private async  Task<Result> StoreDatabase(
        FormDomain formDomain,
        List<SectionDto> sectionsDto, 
        List<QuestionTypeDomain> listQuestionType,
        CancellationToken cancellationToken)
    {
        List<FormSectionDomain> sections = await GetSections(formDomain.Id.Value, cancellationToken);
        var incomingSectionsIds= sectionsDto.Select(x => new FormSectionId(x.Id)).ToList();
        List<FormSectionDomain> sectionsToDesactivate = sections.Where(x => !incomingSectionsIds.Contains(x.Id)).ToList();


        foreach (var sectionToDesactivate in sectionsToDesactivate)
        {
            sectionToDesactivate.Deactivate();
        }
        _unitOfWork.Repository<FormSectionDomain, FormSectionId>().UpdateEntity(sectionsToDesactivate);


        List<QuestionDomain> questions = await GetQuestion(formDomain.Id.Value, cancellationToken);
        var incomingQuestionsIds = sectionsDto.SelectMany(x=>x.Questions).Select(q =>new QuestionId(q.Id)).ToHashSet();
        List<QuestionDomain> questionsToDesactivate = questions.Where(x => !incomingQuestionsIds.Contains(x.Id)).ToList();

        foreach (var questionToDesactivate in questionsToDesactivate)
        {
            questionToDesactivate.Deactivate();
        }
        _unitOfWork.Repository<QuestionDomain, QuestionId>().UpdateEntity(questionsToDesactivate);

        int sectionOrder = 1;
        foreach (var sectionDto in sectionsDto)
        {
            var idSection = new FormSectionId(sectionDto.Id);
            var sectionDomainExisted = sections.Find(x => x.Id == idSection);
            if (sectionDomainExisted is null)
            {
                var formSectionCreatedResult = FormSectionDomain.Create(
                                                        idSection,
                                                        formDomain.Id,
                                                        sectionDto.Title,
                                                        sectionDto.Description,
                                                        sectionOrder
                                                    );
                if (formSectionCreatedResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, formSectionCreatedResult.Errors);
                }
                var formSectionCreated = formSectionCreatedResult.Value;
                _unitOfWork.Repository<FormSectionDomain, FormSectionId>().AddEntity(formSectionCreated);

            }
            else
            {
                sectionDomainExisted.Update(
                                        sectionOrder, 
                                        sectionDto.Title,
                                        sectionDto.Description
                                    );

                _unitOfWork.Repository<FormSectionDomain, FormSectionId>().UpdateEntity(sectionDomainExisted);

            }
            sectionOrder++;
        }



        var questionsSectionDto = sectionsDto
                                    .SelectMany(section =>
                                        section.Questions.Select((q, index) => new QuestionSectionDto
                                        {
                                            IdSection=section.Id,
                                            Question=q,
                                            Order=index+1
                                        })
                                    )
                                    .ToList();

        foreach (var questionSectionDto in questionsSectionDto)
        {
            var questionDto = questionSectionDto.Question;
            var formSectionId = new FormSectionId(questionSectionDto.IdSection);
            int order = questionSectionDto.Order;
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
                var questionCreatedResult = QuestionDomain.Create(
                                                        questionId, 
                                                        formSectionId, 
                                                        questionType.Id, 
                                                        order
                                                    );
                if (questionCreatedResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, questionCreatedResult.Errors);
                }
                var questionCreated= questionCreatedResult.Value;
                _unitOfWork.Repository<QuestionDomain, QuestionId>().AddEntity(questionCreated);


                var resultStoreQuestionAttributeValue = StoreDatabaseQuestionAttributeValue(questionCreated.Id, questionDtoPropertie, questionType);
                if (resultStoreQuestionAttributeValue.IsFailure)
                {
                    return Result.Failure(resultStoreQuestionAttributeValue.ResultType, resultStoreQuestionAttributeValue.Errors);
                }

            }
            else
            {
                questionDomainExisted.Update(order,formSectionId);

                _unitOfWork.Repository<QuestionDomain, QuestionId>().UpdateEntity(questionDomainExisted);
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
                                .Where(qavd => 
                                    !incomingKeys.Any(incomingKey =>
                                            string.Equals(incomingKey, qavd.QuestionTypeAttribute.Attribute.KeyName.Value, StringComparison.OrdinalIgnoreCase)
                                            )
                                    )
                                .ToList();

            foreach (var item in toDisable)
            {
                item.Deactivate();

                _unitOfWork.Repository<QuestionAttributeValueDomain, QuestionAttributeValueId>().UpdateEntity(item);
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
                _unitOfWork.Repository<QuestionAttributeValueDomain, QuestionAttributeValueId>().AddEntity(questionAttributeValueCreated);

            }
            else
            {
                questionAttributeValueExisted.Update(valueToStore);
                _unitOfWork.Repository<QuestionAttributeValueDomain, QuestionAttributeValueId>().UpdateEntity(questionAttributeValueExisted);

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
        var questions = await _formRepository.GetQuestionByIdFormAsync(idForm, false, cancellationToken);
        return questions.ToList();
    }
    private async Task<List<FormSectionDomain>> GetSections(Guid idForm, CancellationToken cancellationToken)
    {
        var sections = await _formRepository.GetSectionsByIdFormAsync(idForm, cancellationToken);
        return sections;
    }
}
