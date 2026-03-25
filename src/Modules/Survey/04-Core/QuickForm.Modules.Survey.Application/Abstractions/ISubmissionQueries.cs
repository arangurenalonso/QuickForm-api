using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Application;

public interface ISubmissionQueries
{
    Task<List<SubmissionExportRow>> GetSubmissionsForExportAsync(
        Guid idForm,
        CancellationToken ct = default);
    Task<ResultT<PaginationResult<RowDto>>> SearchSubmissionsByIdFormAsync(
       Guid idForm,
       List<FiltersForm>? filters,
       int skip = 0,
       int take = 50,
       CancellationToken ct = default
    );
}
