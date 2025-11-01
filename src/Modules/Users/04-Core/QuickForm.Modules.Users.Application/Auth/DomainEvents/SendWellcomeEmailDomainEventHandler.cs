using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

internal sealed class SendWellcomeEmailDomainEventHandler(
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

        throw new NotImplementedException();

    }
}
