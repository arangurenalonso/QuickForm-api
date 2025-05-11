using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record FormSectionTitle
{
    public string Value { get; }

    private FormSectionTitle(string value)
    {
        Value = value;
    }

    public FormSectionTitle()
    {
    }

    public static ResultT<FormSectionTitle> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("FormSectionTitle", "Form Section title cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("FormSectionTitle", "Form Section title must be at most 255 characters long.");
        }

        return new FormSectionTitle(description);
    }

    public static implicit operator string(FormSectionTitle description) => description.Value;
}

