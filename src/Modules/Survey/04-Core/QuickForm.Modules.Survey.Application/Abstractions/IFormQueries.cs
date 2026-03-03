using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

public interface IFormQueries
{
    Task<bool> FormBelongsToCustomerAsync(
    Guid idForm,
    Guid idCustomer,
    CancellationToken ct = default);
    Task<FormViewModel?> GetFormByIdAsync(Guid idForm, CancellationToken ct = default);
    Task<List<FormViewModel>> GetFormsByCustomerIdAsync(Guid idCustomer, CancellationToken ct = default);

    Task<List<FormSectionDomain>> GetStructureFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default);
    Task<List<QuestionForSubmission>> GetQuestionsForSubmissionAsync(
        Guid idForm,
        CancellationToken ct = default);
    Task<List<ColumnDto>> GetFormColumnsByIdFormAsync(Guid idForm, CancellationToken ct = default);
    Task<List<RowDto>> GetFormRowsByIdFormAsync(
            Guid idForm,
            int skip = 0,
            int take = 50,
            CancellationToken ct = default
        );
}
