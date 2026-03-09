using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class ExportFormSubmissionsExcelCommandHandler(
    IDateTimeProvider _dateTimeProvider,
    IFormRepository _formRepository,
    IFormQueries _formQueries,
    ISubmissionQueries _submissionQueries,
    IExcelService _excelService
) : ICommandHandler<ExportFormSubmissionsExcelCommand, FileResultResponse>
{
    public async Task<ResultT<FileResultResponse>> Handle(
        ExportFormSubmissionsExcelCommand request,
        CancellationToken cancellationToken)
    {
        var form = await _formRepository.GetFormToCheckActionAsync(request.IdForm, cancellationToken);
        if (form is null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<FileResultResponse>.FailureT(ResultType.NotFound, error);
        }

        var questions = await _formQueries.GetQuestionsForSubmissionAsync(request.IdForm, cancellationToken);
        var submissions = await _submissionQueries.GetSubmissionsForExportAsync(request.IdForm, cancellationToken);

        var excelResult = _excelService.ExportSubmissionsToExcel(questions, submissions);
        if (excelResult.IsFailure)
        {
            return ResultT<FileResultResponse>.FailureT(excelResult.Errors);
        }

        var now = _dateTimeProvider.UtcNow;
        var safeName = SanitizeFileName(form.Name.Value);
        var fileName = $"{safeName}_submissions_{now:yyyyMMdd_HHmm}.xlsx";

        return new FileResultResponse(
            excelResult.Value,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }

    private static string SanitizeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return string.IsNullOrWhiteSpace(name) ? "form" : name;
    }
}
