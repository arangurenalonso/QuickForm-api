using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed class AuthActionDomainEvent(UserId userId, AuthActionId idAuthAction) : DomainEvent
{
    public UserId UserId { get; init; } = userId;
    public AuthActionId IdAuthAction { get; init; } = idAuthAction;
}

