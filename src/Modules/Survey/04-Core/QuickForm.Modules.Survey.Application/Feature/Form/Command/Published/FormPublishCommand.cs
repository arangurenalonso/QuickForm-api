using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormPublishCommand(
        Guid IdForm,
        List<SectionDto> Sections
    )
    : ICommand<ResultTResponse<FormViewModel>>;
