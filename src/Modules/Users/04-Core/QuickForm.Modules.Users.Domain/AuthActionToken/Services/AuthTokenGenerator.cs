using System.Security.Cryptography;

namespace QuickForm.Modules.Users.Domain;

public static class AuthTokenGenerator
{
    public static string GenerateAlphanumericToken(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return GenerateFromCharset(chars, length);
    }

    public static string GenerateNumericToken(int length = 6)
    {
        const string chars = "0123456789";
        return GenerateFromCharset(chars, length);
    }

    private static string GenerateFromCharset(string chars, int length)
    {
        var token = new char[length];
        using var rng = RandomNumberGenerator.Create();
        var data = new byte[length];
        rng.GetBytes(data);

        for (int i = 0; i < length; i++)
        {
            token[i] = chars[data[i] % chars.Length];
        }

        return new string(token);
    }
}
