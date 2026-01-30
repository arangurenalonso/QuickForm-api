using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record FormSectionsDescription
{
    public string Value { get; }

    private FormSectionsDescription(string value)
    {
        Value = value;
    }

    public FormSectionsDescription()
    {
    }

    public static ResultT<FormSectionsDescription> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return new FormSectionsDescription("");
        }

        var descriptionMaxLength = 1000;
        if (description.Length > descriptionMaxLength)
        {
            return ResultError.InvalidFormat("FormSectionDescription", $"Form Section description must be at most {descriptionMaxLength} characters long.");
        }

        var descriptionTrimmed = description.Trim();

        return new FormSectionsDescription(descriptionTrimmed);
    }

    public static implicit operator string(FormSectionsDescription description) => description.Value;
}
