namespace QuickForm.Modules.Survey.Domain;

public interface IFormRepository
{
    Task<FormDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<FormSectionDomain>> GetStructureFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default);
    Task<List<QuestionDomain>> GetQuestionByIdFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default);
    Task<List<FormSectionDomain>> GetSectionsByIdFormAsync(Guid id,  CancellationToken cancellationToken = default);
}
