namespace QuickForm.Modules.Survey.Domain;

public interface IQuestionTypeRepository
{
    Task<List<QuestionTypeDomain>> GetByTypeKeysAsync(
        List<string> keysName,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);
    Task<List<QuestionTypeDomain>> GetByIdsAsync(
        List<QuestionTypeId> questionTypeIds,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);


}
