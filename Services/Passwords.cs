using System.Security.Cryptography;

namespace ListovkiQuiz.Services;

/// <summary>
/// Turns a password into a "hash" that cannot be turned back into the password.
/// We store the hash. To check a login, we hash what the person typed and compare.
/// </summary>
public static class Passwords
{
    private const int Iterations = 600000;   // more iterations = slower for attackers

    /// <summary>Creates the text we save, in the form: iterations.salt.hash</summary>
    public static string Hash(string password)
    {
        // The salt is random, so two people with the same password get different hashes
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return Iterations + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    /// <summary>True if the typed password matches the saved hash.</summary>
    public static bool Verify(string password, string saved)
    {
        string[] parts = saved.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations))
            return false;

        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expected = Convert.FromBase64String(parts[2]);
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

        // Compares in constant time so the timing does not reveal anything
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
