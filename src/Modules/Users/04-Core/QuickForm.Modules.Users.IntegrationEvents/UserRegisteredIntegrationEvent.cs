using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.IntegrationEvents;
public sealed class UserRegisteredIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }

    public string Email { get; init; }

    public UserRegisteredIntegrationEvent(
        Guid idOutboxMessage,
        DateTime occurredOnUtc,
        Guid userId,
        string email)
        : base(Guid.NewGuid(), occurredOnUtc, idOutboxMessage)
    {
        UserId = userId;
        Email = email;
    }


}
