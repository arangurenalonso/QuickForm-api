using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;
public class FormStructureSectionReponse
{
        public Guid Id {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<FormStructureQuestionReponse> Questions { get; set; }
}

public class FormStructureQuestionReponse
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public JsonElement Properties { get; set; }
}
