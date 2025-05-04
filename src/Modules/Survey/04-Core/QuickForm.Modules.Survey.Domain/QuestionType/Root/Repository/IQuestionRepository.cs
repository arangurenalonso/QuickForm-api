namespace QuickForm.Modules.Survey.Domain;

public interface IQuestionRepository
{
    Task<QuestionDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(QuestionDomain question);
    void Update(QuestionDomain question);
    void Insert(QuestionAttributeValueDomain questionAttributeValue);
    void Update(QuestionAttributeValueDomain questionAttributeValue);
}
