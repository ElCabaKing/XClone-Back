
using System.Security.Cryptography;

namespace Shared.Helpers;

public static class TokenHelper
{
    public static string GenerateRefreshToken(int length = 255)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(length);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, length);
    }
}