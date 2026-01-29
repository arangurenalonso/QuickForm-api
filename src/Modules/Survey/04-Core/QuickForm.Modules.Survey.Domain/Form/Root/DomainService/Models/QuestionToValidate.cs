using System.Text.Json;

namespace QuickForm.Modules.Survey.Domain;
public sealed record QuestionToValidate(
            Guid Id, 
            string Type, 
            JsonElement Properties,
            Dictionary<string, ValidationRule>? Rules
        );
public sealed record ValidationRule(
    JsonElement Value,
    string? Message 
);
