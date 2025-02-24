using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record CreateCustomerCommand(
        Guid CustomerId,
        string Email,
        string FirstName,
        string LastName
    )
    : ICommand; 
