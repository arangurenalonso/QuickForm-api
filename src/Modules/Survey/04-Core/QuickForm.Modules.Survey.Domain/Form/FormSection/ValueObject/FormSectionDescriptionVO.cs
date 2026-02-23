using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record FormSectionDescriptionVO
{
    public string Value { get; private set; } = default!;

    private FormSectionDescriptionVO(string value)
    {
        Value = value;
    }

    public FormSectionDescriptionVO()
    {
    }

    public static ResultT<FormSectionDescriptionVO> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return new FormSectionDescriptionVO("");
        }

        var descriptionMaxLength = 1000;
        if (description.Length > descriptionMaxLength)
        {
            return ResultError.InvalidFormat("FormSectionDescription", $"Form Section description must be at most {descriptionMaxLength} characters long.");
        }

        var descriptionTrimmed = description.Trim();

        return new FormSectionDescriptionVO(descriptionTrimmed);
    }

    public static implicit operator string(FormSectionDescriptionVO description) => description.Value;
}
