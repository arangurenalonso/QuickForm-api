using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using QuickForm.Modules.Users.Application;
using QuickForm.Modules.Users.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuickForm.Common.Infrastructure;

public class TokenService(
    IOptions<JwtOptions> _options,
    IDateTimeProvider _dateTimeProvider
    ) : ITokenService
{

    #region GenerateToken 

    public ResultT<string> GenerateToken(Guid userId,
        string? name,
        string? lastName,
        string? email)
    {
        var claims = CreateClaims(userId,
                                        name,
                                        lastName,
                                        email);

        var signingCredentials = CreateSigningCredentials();
        var tokenDescriptor = CreateJwtSecurityToken(claims, signingCredentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(tokenDescriptor);
    }
    private List<Claim> CreateClaims(
        Guid userId,
        string? name,
        string? lastName,
        string? email)
    {
        
        var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, _options.Value.Subject),
                new(JwtRegisteredClaimNames.Iss, _options.Value.Issuer),
                new(JwtRegisteredClaimNames.Aud, _options.Value.Audience),
                new("userId", userId.ToString())
            };

        if (!string.IsNullOrEmpty(name))
        {
            claims.Add(new Claim("name", name));
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            claims.Add(new Claim("lastName", lastName));
        }

        if (!string.IsNullOrEmpty(email))
        {
            claims.Add(new Claim("email", email));
        }

        return claims;
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
    private JwtSecurityToken CreateJwtSecurityToken(IEnumerable<Claim> claims, SigningCredentials signingCredentials)
    {
        var expires = _dateTimeProvider.UtcNow.AddHours(_options.Value.ExpirationTimeInHours);
        var token = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            expires: expires,
            notBefore: _dateTimeProvider.UtcNow,
            signingCredentials: signingCredentials
        );

        return token;
    }
    #endregion

    #region Validate Token
    public Result ValidateToken(string token)
    {
        var validateSecretKeyAudienceAndIssuerResult = ValidateSecretKeyAudienceAndIssuer(token);
        if (validateSecretKeyAudienceAndIssuerResult.IsFailure)
        {
            return validateSecretKeyAudienceAndIssuerResult.Errors;
        }

        var validateExpirationResult = ValidateExpiration(token);
        if (validateExpirationResult.IsFailure)
        {
            return validateExpirationResult.Errors;
        }

        return Result.Success();
    }
    private Result ValidateSecretKeyAudienceAndIssuer(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_options.Value.SecretKey);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = _options.Value.Issuer,
            ValidAudience = _options.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero,
        };

        try
        {
            tokenHandler.ValidateToken(token, parameters, out _);
            return Result.Success();
        }
        catch (Exception e)
        {
            return CommonMethods.ConvertExceptionToResult(e, "Token");

        }
    }


    private Result ValidateExpiration(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenData = tokenHandler.ReadJwtToken(token);

        var now = _dateTimeProvider.UtcNow;
        var desde = tokenData.ValidFrom;
        var hasta = tokenData.ValidTo;

        var expirationTime = desde.AddHours(_options.Value.ExpirationTimeInHours);
        var firstValid = now < hasta;
        var secondValid = expirationTime == hasta;
        if (!(firstValid && secondValid))
        {
            return ResultError.InvalidInput("Token", "token Expirado");
        }
        return Result.Success();
    }
    #endregion

}
