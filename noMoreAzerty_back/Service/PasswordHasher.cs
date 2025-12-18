using System.Security.Cryptography;

public static class PasswordHasher
{
    /// <summary>
    /// Hache un mot de passe en utilisant PBKDF2 avec SHA-256.
    /// </summary>
    public static string HashPassword(string password, byte[] salt, int iterations = 310_000, int keySize = 32)
    {
        // Création de l'algorithme PBKDF2 avec SHA-256
        // PBKDF2 ralentit volontairement le calcul pour contrer les attaques par force brute
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256
        );

        // Génère la clé dérivée et la convertit en Base64 pour le stockage
        return Convert.ToBase64String(pbkdf2.GetBytes(keySize));
    }

    /// <summary>
    /// Compare deux chaînes hachées de manière sécurisée (temps constant).
    /// </summary>
    public static bool SecureEquals(string a, string b)
    {
        // Compare les deux tableaux d'octets en temps constant
        // Empêche les attaques par analyse du temps d'exécution (timing attacks)
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(a),
            Convert.FromBase64String(b)
        );
    }
}
