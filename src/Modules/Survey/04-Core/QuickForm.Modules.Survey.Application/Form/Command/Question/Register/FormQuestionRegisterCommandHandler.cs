﻿using MediatR;
using System.Threading;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using System.Text.Json;
using System.Globalization;

namespace QuickForm.Modules.Survey.Application;


internal sealed class FormQuestionRegisterCommandHandler(
    IFormRepository _formRepository,
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
        return ResultTResponse<Guid>.Success(Guid.NewGuid(), $".");


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
            var validatePropertiesResult = Validate(questionType, properties);
            if (validatePropertiesResult.IsFailure)
            {
                return Result.Failure(ResultType.NotFound, validatePropertiesResult.Errors);
            }
            // Validate properties based on the question type
            // Implement your validation logic here
        }

        return Result.Success();
    }
    private Result Validate(QuestionTypeDomain questionTypeDomain, JsonElement properties)
    {
        foreach (var property in properties.EnumerateObject())
        {
            string propertyName = property.Name;
            QuestionTypeAttributeDomain? questionTypeAttribute=questionTypeDomain.QuestionTypeAttributes
                                                                        .FirstOrDefault(x => x.Attribute.KeyName== propertyName);
            if (questionTypeAttribute is null)
            {
                var errorQuestion = ResultError.InvalidInput(
                    "QuestionTypeAttribute",
                    $"The following question type attribute dosen't belong to the questionType {questionTypeDomain.KeyName}: {propertyName} ."
                );
                return Result.Failure(ResultType.NotFound, errorQuestion);
            }
            JsonElement value = property.Value;
            var dataType = questionTypeAttribute.Attribute.DataType.Description.Value.ToLowerInvariant();

            bool isValid = dataType switch
            {
                "string" => value.ValueKind == JsonValueKind.String,
                "int" => value.TryGetInt32(out _),
                "decimal" => value.TryGetDecimal(out _),
                "bool" => value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False,
                "datetime" =>
                    value.ValueKind == JsonValueKind.String
                        ? DateTime.TryParse(value.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                        : DateTime.TryParse(value.GetRawText(), CultureInfo.InvariantCulture, DateTimeStyles.None, out _),
                _ => false
            };


            if (!isValid)
            {
                var errorQuestion = ResultError.InvalidInput(
                    "DataType",
                    $"Invalid data type for attribute '{propertyName}'. Expected '{dataType}', but received '{value.ValueKind}'."
                );
                return Result.Failure(ResultType.Validation, errorQuestion);
            }
        }
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
}
