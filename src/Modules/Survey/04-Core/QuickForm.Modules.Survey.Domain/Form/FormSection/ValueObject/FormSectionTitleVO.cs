using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record FormSectionTitleVO
{
    public string Value { get; private set; } = default!;

    private FormSectionTitleVO(string value)
    {
        Value = value;
    }

    public FormSectionTitleVO()
    {
    }

    public static ResultT<FormSectionTitleVO> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("FormSectionTitle", "Form Section title cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("FormSectionTitle", "Form Section title must be at most 255 characters long.");
        }

        return new FormSectionTitleVO(description);
    }

    public static implicit operator string(FormSectionTitleVO description) => description.Value;
}

