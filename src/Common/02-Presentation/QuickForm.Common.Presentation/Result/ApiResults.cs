using Microsoft.AspNetCore.Http;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Presentation;
public static class ApiResults
{
    public static IResult Problem(this Result result)
    {
        if (result.IsSuccess || result.ResultType == ResultType.None)
        {
            throw new InvalidOperationException();
        }

        var response = new ResultResponse
        {
            StatusCode = GetStatusCode(result.ResultType),
            Title = GetTitle(result.ResultType),
            Errors = result.Errors.ToList()
        };

        return Results.Json(response, statusCode: response.StatusCode);
    }
    private static int GetStatusCode(ResultType errorType) =>
           errorType switch
           {
               ResultType.FluentValidation => StatusCodes.Status400BadRequest,
               ResultType.DomainValidation => StatusCodes.Status400BadRequest,
               ResultType.ModelDataValidation => StatusCodes.Status400BadRequest,
               ResultType.MismatchValidation => StatusCodes.Status400BadRequest,
               ResultType.NotFound => StatusCodes.Status404NotFound, 
               ResultType.Conflict => StatusCodes.Status409Conflict,
               ResultType.InternalServerError => StatusCodes.Status500InternalServerError,
               ResultType.Unauthorized => StatusCodes.Status401Unauthorized,
               ResultType.Forbidden => StatusCodes.Status403Forbidden,
               _ => StatusCodes.Status500InternalServerError
           };

    private static string GetTitle(ResultType errorType) =>
        errorType switch
        {
            ResultType.FluentValidation => "Bad Request - Fluent Validation",
            ResultType.DomainValidation => "Bad Request - Domain Validation",
            ResultType.ModelDataValidation => "Bad Request - Invalid Model Data",
            ResultType.MismatchValidation => "Bad Request - Mismatch Validation",
            ResultType.NotFound => "Not Found",
            ResultType.Conflict => "Conflict",
            ResultType.InternalServerError => "Internal Server Error",
            ResultType.Unauthorized => "Unauthorized",
            ResultType.Forbidden => "Forbidden",
            _ => "Internal Server Error - Unspecified Error"
        };
}

public class ResultResponse
{
    public int StatusCode { get; set; }
    public string? Title { get; set; } = "";
    public List<ResultError> Errors { get; set; } = new List<ResultError>();
}
