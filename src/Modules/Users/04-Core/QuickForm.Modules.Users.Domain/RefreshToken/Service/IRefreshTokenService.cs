using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public interface IRefreshTokenService
{
    ResultT<string> Hash(string token);
    string Generate();
    int ExpirationDays();
}
