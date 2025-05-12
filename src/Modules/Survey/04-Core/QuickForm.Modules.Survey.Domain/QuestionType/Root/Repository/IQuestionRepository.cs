namespace QuickForm.Modules.Survey.Domain;

public interface IQuestionRepository
{
    Task<QuestionDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
