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
            return ResultError.EmptyValue("FormSectionDescription", "Form Section description cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("FormSectionDescription", "Form Section description must be at most 255 characters long.");
        }

        return new FormSectionsDescription(description);
    }

    public static implicit operator string(FormSectionsDescription description) => description.Value;
}
