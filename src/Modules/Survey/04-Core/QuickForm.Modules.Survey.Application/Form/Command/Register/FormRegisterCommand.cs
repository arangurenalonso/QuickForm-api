using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record FormRegisterCommand(
        string Name, 
        string? Description
    )
    : ICommand<Guid>;
