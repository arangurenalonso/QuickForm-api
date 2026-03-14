using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using QuickForm.Modules.Users.Application;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuickForm.Common.Infrastructure;

public class TokenService(
    IOptions<JwtOptions> _options,
    IDateTimeProvider _dateTimeProvider
) : ITokenService
{
    public ResultT<string> GenerateToken(Guid userId, string? email)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return ResultError.InvalidInput("UserId", "UserId cannot be empty.");
            }

            var now = _dateTimeProvider.UtcNow;
            var claims = CreateClaims(userId, email);
            var signingCredentials = CreateSigningCredentials();

            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddHours(_options.Value.ExpirationTimeInHours),
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        catch (Exception e)
        {
            return CommonMethods.ConvertExceptionToResult(e, "Token");
        }
    }

    private List<Claim> CreateClaims(Guid userId, string? email)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("userId", userId.ToString())
        };

        if (!string.IsNullOrWhiteSpace(email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        }

        return claims;
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var secret = _options.Value.SecretKey;

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(secret);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException("JWT SecretKey must be at least 32 bytes long.");
        }

        var key = new SymmetricSecurityKey(keyBytes);
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public Result ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return ResultError.EmptyValue("Token", "Token cannot be null or empty.");
        }

        try
        {
            var secret = _options.Value.SecretKey;

            if (string.IsNullOrWhiteSpace(secret))
            {
                return ResultError.InvalidOperation("Token", "JWT SecretKey is not configured.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(secret);

            if (keyBytes.Length < 32)
            {
                return ResultError.InvalidOperation("Token", "JWT SecretKey must be at least 32 bytes long.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = _options.Value.Issuer,
                ValidAudience = _options.Value.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, parameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                return ResultError.InvalidInput("Token", "Invalid JWT token.");
            }

            if (!string.Equals(jwtToken.Header.Alg, SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
            {
                return ResultError.InvalidInput("Token", "Invalid token algorithm.");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            return CommonMethods.ConvertExceptionToResult(e, "Token");
        }
    }
}
