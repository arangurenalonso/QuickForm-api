using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed class UserRegisteredDomainEvent(UserId userId) : DomainEvent
{
    public UserId UserId { get; init; } = userId;
}
