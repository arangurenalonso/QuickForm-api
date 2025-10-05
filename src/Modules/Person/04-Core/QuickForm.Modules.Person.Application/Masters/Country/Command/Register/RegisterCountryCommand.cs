using QuickForm.Common.Application;

namespace QuickForm.Modules.Person.Application;
public sealed record RegisterCountryCommand(
        string Name,
        string? Description
    )
    : ICommand<ResultResponse>;
