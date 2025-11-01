using System.Security.Cryptography;

namespace QuickForm.Modules.Users.Domain;
public static class AuthTokenGenerator
{
    public static string GenerateAlphanumericToken(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var token = new char[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            var data = new byte[length];
            rng.GetBytes(data);

            for (int i = 0; i < token.Length; i++)
            {
                token[i] = chars[data[i] % chars.Length];
            }
        }
        var tokenString = $"{new string(token)}";

        return tokenString;
    }
}
