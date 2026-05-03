using PosTechChallenge.Aplicacao.Utils;
using Xunit;

namespace PosTechChallenge.Testes.Application;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_SenhaValida_DeveGerarHashVerificavel()
    {
        var hash = PasswordHasher.HashPassword("Senha@123");

        Assert.True(PasswordHasher.VerifyPassword("Senha@123", hash));
    }

    [Fact]
    public void HashPassword_SenhaVazia_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => PasswordHasher.HashPassword(""));
    }

    [Fact]
    public void VerifyPassword_SenhaIncorreta_DeveRetornarFalse()
    {
        var hash = PasswordHasher.HashPassword("Senha@123");

        Assert.False(PasswordHasher.VerifyPassword("Outra@123", hash));
    }

    [Fact]
    public void VerifyPassword_HashInvalido_DeveRetornarFalse()
    {
        Assert.False(PasswordHasher.VerifyPassword("Senha@123", "hash-invalido"));
    }
}
