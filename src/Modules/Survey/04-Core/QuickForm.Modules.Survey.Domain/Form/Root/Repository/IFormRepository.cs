namespace QuickForm.Modules.Survey.Domain;
public interface IFormRepository
{
    Task<FormDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FormDomain?> GetFormToCheckActionAsync(Guid id, CancellationToken cancellationToken = default);

}
