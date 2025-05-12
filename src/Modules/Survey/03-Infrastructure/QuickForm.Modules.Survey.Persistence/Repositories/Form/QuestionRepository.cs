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

}
