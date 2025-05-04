using System.Text.Json;

namespace QuickForm.Modules.Survey.Application;
public class AttributeValueOccurrence
{
    public Guid IdQuestionType { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public JsonElement Value { get; set; }
}
