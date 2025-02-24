using QuickForm.Common.Application;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

internal sealed class SendWellcomeEmailDomainEventHandler()
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

    }
}
