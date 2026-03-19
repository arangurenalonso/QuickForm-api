using System.Text.Json;

namespace QuickForm.Modules.Survey.Presentation;


internal sealed class FormSectionRegisterDtoRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<FormQuestionRegisterRequestDto> Fields { get; set; } = new();

}

internal sealed class FormQuestionRegisterRequestDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;

    // Usamos JsonElement para representar datos dinámicos (más eficiente que Dictionary<string, object>)
    public JsonElement Properties { get; set; }
    public Dictionary<string, RuleRequestDto>? Rules { get; set; } = new();
}
internal sealed class RuleRequestDto
{
    public JsonElement Value { get; set; }   // admite bool, number, string, etc.
    public string? MessageTemplate { get; set; } = string.Empty;
}
