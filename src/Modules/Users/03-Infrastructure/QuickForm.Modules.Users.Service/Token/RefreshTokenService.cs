using System.Security.Cryptography;
using System.Text;
using MassTransit.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Service;

public sealed class RefreshTokenService(
        IOptions<RefreshTokenOptions> _options
    ) : IRefreshTokenService
{
    public string Generate()
    {
        var lengthInBytes = _options.Value.LengthInBytes;

        if (lengthInBytes <= 0)
        {
            throw new InvalidOperationException("Refresh token length must be greater than zero.");
        }
        var bytes = new byte[lengthInBytes];
        RandomNumberGenerator.Fill(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    public int ExpirationDays()
    {
        return _options.Value.ExpirationDays;
    }
    public ResultT<string> Hash(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResultError.EmptyValue("RefreshToken", "Refresh token cannot be null or empty.");
            }

            var secret = _options.Value.SecretKey;

            if (string.IsNullOrWhiteSpace(secret))
            {
                return ResultError.InvalidOperation("RefreshTokenHash", "Refresh token hashing secret is not configured.");
            }

            var secretBytes = Encoding.UTF8.GetBytes(secret);

            if (secretBytes.Length < 32)
            {
                return ResultError.InvalidOperation("RefreshTokenHash", "Refresh token hashing secret must be at least 32 bytes long.");
            }

            using var hmac = new HMACSHA256(secretBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token.Trim()));
            var hash = Convert.ToHexString(hashBytes);

            return hash;
        }
        catch (Exception ex)
        {
            return ResultError.InvalidOperation("RefreshTokenHash", ex.Message);
        }
    }

}
