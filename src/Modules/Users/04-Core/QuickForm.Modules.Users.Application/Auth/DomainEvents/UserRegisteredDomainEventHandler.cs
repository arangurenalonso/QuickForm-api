using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.IntegrationEvents;

namespace QuickForm.Modules.Users.Application;
internal sealed class UserRegisteredDomainEventHandler(
    IEventBus bus, 
    IUserDapperRepository _userDapperRepository
    ) : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        UserResponse? user = await _userDapperRepository.GetUserById(domainEvent.UserId.Value);

        if (user is null)
        {
            var error = ResultError.NullValue("User", $"User with Id '{domainEvent.UserId.Value}' Not Found");
            throw new QuickFormException(nameof(UserRegisteredDomainEventHandler), error);
        }

        await bus.PublishAsync(
            new UserRegisteredIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                user.Id,
                user.Email),
            cancellationToken);
    }
}
