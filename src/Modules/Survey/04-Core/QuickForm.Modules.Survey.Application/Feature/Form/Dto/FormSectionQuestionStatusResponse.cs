namespace QuickForm.Modules.Survey.Application;
public sealed class FormSectionQuestionStatusResponse
{
    public bool HasSections => Sections.Count > 0;
    public bool AllSectionsHaveQuestions  => Sections.Count > 0 && Sections.TrueForAll(section => section.HasQuestions);
    public List<SectionQuestionStatusResponse> Sections { get; init; } = [];
}

public sealed class SectionQuestionStatusResponse
{
    public Guid SectionId { get; init; }
    public string SectionName { get; init; } = string.Empty;
    public bool HasQuestions { get; init; }
}
