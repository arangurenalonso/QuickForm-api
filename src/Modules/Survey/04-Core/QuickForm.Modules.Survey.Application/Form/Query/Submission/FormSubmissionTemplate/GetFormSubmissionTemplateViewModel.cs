namespace QuickForm.Modules.Survey.Application;
public sealed record GetFormSubmissionTemplateViewModel(
        FormViewModel Form,
        List<FormStructureSectionViewModel> Sections
    );
