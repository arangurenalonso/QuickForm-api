namespace QuickForm.Modules.Survey.Application;

public interface ISubmissionQueries
{
    Task<List<SubmissionExportRow>> GetSubmissionsForExportAsync(
        Guid idForm,
        CancellationToken ct = default);
}
