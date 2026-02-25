using ClosedXML.Excel;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Service;

public sealed class ExcelService : IExcelService
{
    public ResultT<byte[]> ExportSubmissionsToExcel(
        IReadOnlyList<QuestionForSubmission> questions,
        IReadOnlyList<SubmissionExportRow> submissions)
    {
        var columnsResult = BuildColumns(questions);
        if (columnsResult.IsFailure)
        {
            return ResultT<byte[]>.FailureT(columnsResult.Errors);
        }

        var bytes = BuildExcel(columnsResult.Value, submissions);
        return bytes;
    }

    private static byte[] BuildExcel(
        IReadOnlyList<(Guid QuestionId, string Name)> columns,
        IReadOnlyList<SubmissionExportRow> submissions)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Submissions");

        // Header
        var col = 1;
        ws.Cell(1, col++).Value = "SubmissionId";
        ws.Cell(1, col++).Value = "SubmittedAtUtc";

        foreach (var c in columns)
        {
            ws.Cell(1, col++).Value = c.Name;
        }

        // Rows
        var row = 2;
        foreach (var s in submissions)
        {
            col = 1;
            ws.Cell(row, col++).Value = s.SubmissionId.ToString();
            ws.Cell(row, col++).Value = s.SubmittedAtUtc.ToString("O");

            var valuesByQuestionId = s.Values.ToDictionary(v => v.QuestionId, v => v.RawJsonValue);

            foreach (var c in columns)
            {
                if (!valuesByQuestionId.TryGetValue(c.QuestionId, out var raw))
                {
                    ws.Cell(row, col++).Value = "";
                    continue;
                }

                ws.Cell(row, col++).Value = ExcelValueFormatter.ToExcelText(raw);
            }

            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static ResultT<List<(Guid QuestionId, string Name)>> BuildColumns(
        IReadOnlyList<QuestionForSubmission> questions)
    {
        var nameAttrId = AttributeType.Name.GetId();
        var cols = new List<(Guid QuestionId, string Name)>();

        foreach (var q in questions)
        {
            var nameAttr = q.Attributes.FirstOrDefault(a => a.AttributeId == nameAttrId);

            if (string.IsNullOrWhiteSpace(nameAttr.Value))
            {
                var err = ResultError.InvalidInput(
                    "FormDefinition",
                    $"Question '{q.QuestionId.Value}' is missing a valid Name attribute."
                );
                return ResultT<List<(Guid QuestionId, string Name)>>.FailureT(ResultType.DomainValidation, err);
            }

            cols.Add((q.QuestionId.Value, nameAttr.Value.Trim()));
        }

        cols = cols.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase).ToList();

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in cols)
        {
            if (!seen.Add(c.Name))
            {
                var err = ResultError.InvalidInput(
                    "FormDefinition",
                    $"Duplicate question Name '{c.Name}' found. Export headers must be unique."
                );
                return ResultT<List<(Guid QuestionId, string Name)>>.FailureT(ResultType.DomainValidation, err);
            }
        }

        return cols;
    }
}
