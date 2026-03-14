using Microsoft.Extensions.Options;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using System.Security.Cryptography;
using System.Text;

namespace QuickForm.Modules.Users.Service;

public sealed class AuthActionTokenHashingService(
    IOptions<AuthActionTokenHashingOptions> _options
) : IAuthActionTokenHashingService
{
    public ResultT<string> Hash(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResultError.EmptyValue("Token", "Token cannot be null or empty.");
            }

            var secret = _options.Value.SecretKey;

            if (string.IsNullOrWhiteSpace(secret))
            {
                return ResultError.InvalidOperation("TokenHash", "Auth action token hashing secret is not configured.");
            }

            var secretBytes = Encoding.UTF8.GetBytes(secret);

            if (secretBytes.Length < 32)
            {
                return ResultError.InvalidOperation("TokenHash", "Auth action token hashing secret must be at least 32 bytes long.");
            }

            var normalized = token.Trim();

            using var hmac = new HMACSHA256(secretBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(normalized));
            var hash = Convert.ToHexString(hashBytes);

            return hash;
        }
        catch (Exception ex)
        {
            return ResultError.InvalidOperation("TokenHash", ex.Message);
        }
    }

    public Result Verify(string token, string storedHash)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResultError.EmptyValue("Token", "Token cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(storedHash))
            {
                return ResultError.EmptyValue("TokenHash", "Stored hash cannot be null or empty.");
            }

            var currentHashResult = Hash(token);
            if (currentHashResult.IsFailure)
            {
                return currentHashResult.Errors;
            }

            var currentHashBytes = Encoding.UTF8.GetBytes(currentHashResult.Value);
            var storedHashBytes = Encoding.UTF8.GetBytes(storedHash.Trim().ToUpperInvariant());

            if (currentHashBytes.Length != storedHashBytes.Length)
            {
                return ResultError.InvalidOperation("Token", "Invalid token.");
            }

            var isValid = CryptographicOperations.FixedTimeEquals(currentHashBytes, storedHashBytes);

            if (!isValid)
            {
                return ResultError.InvalidOperation("Token", "Invalid token.");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return ResultError.InvalidOperation("Token", ex.Message);
        }
    }
}
