using QuickForm.Common.Domain;

namespace QuickForm.Common.Application;
public interface ICurrentUserService
{
    ResultT<Guid> UserId { get; } 
    string AuthenticationToken { get; }
    List<string> Roles { get; }
}
