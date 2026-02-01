using System.Text.Json;
using System.Text.RegularExpressions;

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
    public Dictionary<string, RuleQuestionResponseDto> Rules { get; set; } = new();

}

public class RuleQuestionResponseDto
{
    public JsonElement Value { get; set; }
    public string Message => ApplyTemplate();
    public string MessageTemplate { get; set; }
    public string? Placeholder { get; set; }
    private string ApplyTemplate()
    {
        if (string.IsNullOrWhiteSpace(MessageTemplate))
        {
            return string.Empty;
        }
        if (string.IsNullOrWhiteSpace(Placeholder))
        {
            return MessageTemplate;
        }
        string valueToReplace = "";
        switch (Value.ValueKind)
        {
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                return MessageTemplate;
            case JsonValueKind.String:
                valueToReplace = Value.GetString() ?? string.Empty;
                break;
            case JsonValueKind.Number:
                valueToReplace = Value.ToString() ?? string.Empty;
                break;
            case JsonValueKind.True:
                valueToReplace = "true";
                break;
            case JsonValueKind.False:
                valueToReplace = "false";
                break;
            default:
                throw new ArgumentException("Invalid JsonValueKind");
        }

        return Regex.Replace(MessageTemplate, Placeholder, valueToReplace);
    }


}
