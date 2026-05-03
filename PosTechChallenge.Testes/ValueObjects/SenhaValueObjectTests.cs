using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Testes.ValueObjects;

public class SenhaValueObjectTests
{
    [Fact]
    public void Construtor_SenhaValida_DeveCriarObjeto()
    {
        var senha = new SenhaValueObject("Senha@123");
        Assert.Equal("Senha@123", senha.Valor);
    }

    [Fact]
    public void Construtor_SenhaVazia_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new SenhaValueObject(string.Empty));
        Assert.Contains("vazia", ex.Message);
    }

    [Fact]
    public void Construtor_SenhaNula_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SenhaValueObject(null!));
    }

    [Fact]
    public void Construtor_SenhaComMenosDe8Caracteres_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new SenhaValueObject("Ab1@567"));
        Assert.Contains("8 caracteres", ex.Message);
    }

    [Fact]
    public void Construtor_SenhaSemLetraMaiuscula_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new SenhaValueObject("senha@123"));
        Assert.Contains("maiúscula", ex.Message);
    }

    [Fact]
    public void Construtor_SenhaComExatamente8Caracteres_DeveCriarObjeto()
    {
        var senha = new SenhaValueObject("Senha@12");
        Assert.Equal("Senha@12", senha.Valor);
    }

    [Fact]
    public void ConfirmarSenha_SenhasIguais_DeveRetornarTrue()
    {
        var senha = new SenhaValueObject("Senha@123");
        Assert.True(senha.ConfirmarSenha("Senha@123"));
    }

    [Fact]
    public void ConfirmarSenha_SenhasDiferentes_DeveRetornarFalse()
    {
        var senha = new SenhaValueObject("Senha@123");
        Assert.False(senha.ConfirmarSenha("Senha@456"));
    }

    [Fact]
    public void ConfirmarSenha_ConfirmacaoVazia_DeveRetornarFalse()
    {
        var senha = new SenhaValueObject("Senha@123");
        Assert.False(senha.ConfirmarSenha(string.Empty));
    }
}
