using BCrypt.Net;

namespace PosTechChallenge.Aplicacao.Utils;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia.", nameof(password));
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA384);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;
        try
        {
            return BCrypt.Net.BCrypt.EnhancedVerifyPassword(password, hash);
        }
        catch
        {
            return false;
        }
    }
}