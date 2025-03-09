using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.IntegrationEvents;
public sealed class UserRegisteredIntegrationEvent : IntegrationEvent
{
    public UserRegisteredIntegrationEvent(
        Guid idDomainEvent,
        DateTime occurredOnUtc,
        Guid userId,
        string email,
        string firstName,
        string lastName)
        : base(Guid.NewGuid(), occurredOnUtc, idDomainEvent)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid UserId { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }
}
