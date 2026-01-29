using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class SaveFormStructureCommandHandler(
    IUnitOfWork _unitOfWork,
    IFormRepository _formRepository,
    IQuestionTypeRepository _questionTypeRepository,
    ICurrentUserService _currentUserService,
    ICustomerRepository _customerRepository
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


        var questionsTypeResult = await GetQuestionType(questions, cancellationToken);
        if (questionsTypeResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, questionsTypeResult.Errors);
        }

        List<QuestionTypeDomain> questionsType = questionsTypeResult.Value;

        var questionsDto = questions.Select(q =>
                                        new QuestionToValidate(
                                                q.Id, 
                                                q.Type, 
                                                q.Properties,
                                                q.Rules?.ToDictionary(
                                                        kvp => kvp.Key,
                                                        kvp => new ValidationRule(
                                                            kvp.Value.Value,
                                                            kvp.Value.Message ?? string.Empty
                                                        )
                                                    )
                                                )
                                        ).ToList();

        var validatePropertiesResult = new QuestionValidationService().Validate(questionsDto, questionsType);
        if (validatePropertiesResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, validatePropertiesResult.Errors);
        }

        var incomingSections = request.Sections
                                        .Select(section => (
                                            section.Id,
                                            section.Title,
                                            section.Description,
                                            section.Questions
                                                .Select(q => (
                                                    q.Id,
                                                    q.Properties,
                                                    q.Rules?.ToDictionary(
                                                            kvp => kvp.Key,
                                                            kvp => new ValidationRule(
                                                                kvp.Value.Value,
                                                                kvp.Value.Message ?? string.Empty
                                                            )
                                                        ),
                                                    questionsType.First( qt => qt.KeyName.Value == q.Type)
                                                ))
                                                .ToList()
                                        ))
                                        .ToList();

        var applyResult = formDomain.ApplySectionsChanges(incomingSections);
        if (applyResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(applyResult.ResultType, applyResult.Errors);
        }

        _unitOfWork.Repository<FormDomain, FormId>().UpdateEntity(formDomain);

        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);

        if (resultTransaction.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultTransaction.ResultType, resultTransaction.Errors);
        }

        return ResultTResponse<Guid>.Success(Guid.NewGuid(), $".");
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
        var statusValidToUpdate = new[]
        {
            new MasterId(FormStatusType.Draft.GetId()),
            new MasterId(FormStatusType.Paused.GetId()),
        };

        if (!statusValidToUpdate.Contains(form.IdStatus))
        {
            var errorForm = ResultError.InvalidOperation(
            "FormStatus",
            $"The form with ID '{idForm}' is in a status that does not allow updates. Current status ID: '{form.IdStatus.Value}'."
            );
            return ResultT<FormDomain>.FailureT(ResultType.DomainValidation, errorForm);
        }

        return form;

    }
}
