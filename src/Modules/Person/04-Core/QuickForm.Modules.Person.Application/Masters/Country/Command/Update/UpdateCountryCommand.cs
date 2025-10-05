using QuickForm.Common.Application;

namespace QuickForm.Modules.Person.Application;
public sealed record UpdateCountryCommand(
        Guid Id,
        string Name,
        string? Description
    )
    : ICommand<ResultResponse>;

