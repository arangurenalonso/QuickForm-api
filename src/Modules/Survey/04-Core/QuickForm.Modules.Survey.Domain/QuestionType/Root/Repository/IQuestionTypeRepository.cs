namespace QuickForm.Modules.Survey.Domain;

public interface IQuestionTypeRepository
{
    Task<List<QuestionTypeDomain>> GetByTypeKeysAsync(
        List<string> keysName,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

}
