using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public interface IAuthActionTokenHashingService
{
    ResultT<string> Hash(string token);
    Result Verify(string token, string storedHash);
}
