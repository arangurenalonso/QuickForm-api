using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

public interface IExcelService
{
    ResultT<byte[]> ExportSubmissionsToExcel(
        IReadOnlyList<QuestionForSubmission> questions,
        IReadOnlyList<SubmissionExportRow> submissions
    );
}
