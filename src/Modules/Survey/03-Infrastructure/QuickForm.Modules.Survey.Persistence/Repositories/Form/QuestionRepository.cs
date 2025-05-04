using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public class QuestionRepository(
    SurveyDbContext _context
    ) : IQuestionRepository
{

    public async Task<QuestionDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        QuestionId questionId = new QuestionId(id);
        return await _context.Question.FirstOrDefaultAsync(u => u.Id == questionId && u.IsActive, cancellationToken);
    }

    public void Insert(QuestionDomain question)
    {
        _context.Question.Add(question);
    }
    public void Update(QuestionDomain question)
    {
        _context.Question.Update(question);
    }
    public void Insert(QuestionAttributeValueDomain questionAttributeValue)
    {
        _context.QuestionAttributeValue.Add(questionAttributeValue);
    }
    public void Update(QuestionAttributeValueDomain questionAttributeValue)
    {
        _context.QuestionAttributeValue.Update(questionAttributeValue);
    }
}
