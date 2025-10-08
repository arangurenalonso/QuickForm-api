using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed class AuthActionDomainEvent(UserId userId, MasterId idAuthAction) : DomainEvent
{
    public UserId UserId { get; init; } = userId;
    public MasterId IdAuthAction { get; init; } = idAuthAction;
}

