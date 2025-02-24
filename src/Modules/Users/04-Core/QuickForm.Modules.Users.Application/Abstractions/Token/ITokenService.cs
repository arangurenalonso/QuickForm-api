using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;
public interface ITokenService
{
    ResultT<string> GenerateToken(
        Guid userId,
        string? name,
        string? lastName,
        string? email);
    Result ValidateToken(string token);
}
