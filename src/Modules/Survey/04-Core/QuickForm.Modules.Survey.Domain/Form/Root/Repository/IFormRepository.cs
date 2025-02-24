namespace QuickForm.Modules.Survey.Domain.Form;

public interface IFormRepository
{
    Task<FormDomain?> GetAsync(FormId id, CancellationToken cancellationToken = default);
    void Insert(FormDomain form);
}
