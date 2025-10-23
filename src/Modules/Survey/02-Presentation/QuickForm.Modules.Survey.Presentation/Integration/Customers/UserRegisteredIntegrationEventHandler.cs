using MediatR;
using QuickForm.Common.Application;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Users.IntegrationEvents;

namespace QuickForm.Modules.Survey.Presentation;
internal sealed class UserRegisteredIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public override async Task Handle(
        UserRegisteredIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCustomerCommand(
                integrationEvent.UserId,
                integrationEvent.Email);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new QuickFormException(nameof(CreateCustomerCommand), result.Errors[0]);
        }
    }
}
