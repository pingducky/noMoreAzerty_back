using System.Security.Cryptography;

public static class PasswordHasher
{
    public static string HashPassword(string password, byte[] salt, int iterations = 310_000, int keySize = 32)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(pbkdf2.GetBytes(keySize));
    }

    public static bool SecureEquals(string a, string b)
    {
        return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(a), Convert.FromBase64String(b));
    }
}
