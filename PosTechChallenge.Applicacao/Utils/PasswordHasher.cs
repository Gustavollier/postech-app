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
        try
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
