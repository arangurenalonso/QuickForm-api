using Microsoft.AspNetCore.Http;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public class CurrentUserService(IHttpContextAccessor _httpContextAccessor) : ICurrentUserService
{
    public ResultT<Guid> UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetClaim("userId") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid userIdGuid))
            {

                var error = ResultError.InvalidInput("UserId", $"User ID not found or invalid. Ensure the authentication token includes 'userId' in its claims");
                return ResultT<Guid>.FailureT(ResultType.NotFound, error);
            }

            return userIdGuid;
        }
    }
    public string UserFullName
    {
        get
        {
            var name= _httpContextAccessor.HttpContext?.User.GetClaim("name") ?? string.Empty;
            var lastName = _httpContextAccessor.HttpContext?.User.GetClaim("lastName") ?? string.Empty;
            return $"{name} {lastName}";
        }
    }
    public List<string> Roles => _httpContextAccessor?.HttpContext?.User?.FindAll("rol").Select(c => c.Value).ToList() ?? [];
    public string AuthenticationToken
    {
        get
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return string.Empty;
            }
            const string bearerPrefix = "Bearer ";
            var token = authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
                ? authorizationHeader.Substring(bearerPrefix.Length).Trim()
                : string.Empty;

            return token;
        }
    }


}
