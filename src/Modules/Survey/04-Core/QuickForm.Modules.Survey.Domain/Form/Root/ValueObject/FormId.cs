namespace QuickForm.Modules.Survey.Domain.Form;

public sealed record FormId(Guid Value)
{
    public static FormId Create() => new FormId(Guid.NewGuid());
}
