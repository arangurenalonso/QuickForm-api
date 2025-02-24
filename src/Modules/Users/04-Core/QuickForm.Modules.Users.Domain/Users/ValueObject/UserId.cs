
namespace QuickForm.Modules.Users.Domain;

public sealed record UserId(Guid Value)
{
    public static UserId Create() => new UserId(Guid.NewGuid());
}
