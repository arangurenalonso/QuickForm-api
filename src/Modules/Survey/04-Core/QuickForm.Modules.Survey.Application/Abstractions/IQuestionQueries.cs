namespace QuickForm.Modules.Survey.Application;

public interface IQuestionQueries
{
    Task<List<QuestionTypeFiltersGroupDto>> GetAllQuestionTypeFiltersAsync(
        CancellationToken ct = default
    );
}
