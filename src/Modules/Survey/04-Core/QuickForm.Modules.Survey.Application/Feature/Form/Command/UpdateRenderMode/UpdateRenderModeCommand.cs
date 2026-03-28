using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record UpdateRenderModeCommand(
        Guid IdForm,
        Guid IdTypeRender
    )
    : ICommand<ResultResponse>;
