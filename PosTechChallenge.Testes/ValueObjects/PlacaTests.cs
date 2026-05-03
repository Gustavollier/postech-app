using PosTechChallenge.Dominio.Model;
using Xunit;

namespace PosTechChallenge.Testes.ValueObjects;

public class PlacaTests
{
    [Fact]
    public void Construtor_PlacaMercosulValida_DeveCriarObjeto()
    {
        // Formato Mercosul: 3 letras + 1 dígito + 1 letra + 2 dígitos
        var placa = new Placa("ABC1D23");
        Assert.Equal("ABC1D23", placa.Valor);
    }

    [Fact]
    public void Construtor_PlacaAntigaValida_DeveCriarObjeto()
    {
        // Formato antigo: 3 letras + 4 dígitos
        var placa = new Placa("ABC1234");
        Assert.Equal("ABC1234", placa.Valor);
    }

    [Fact]
    public void Construtor_PlacaEmMinusculas_DeveNormalizarParaMaiusculas()
    {
        var placa = new Placa("abc1d23");
        Assert.Equal("ABC1D23", placa.Valor);
    }

    [Fact]
    public void Construtor_PlacaComEspacos_DeveNormalizarEValidar()
    {
        var placa = new Placa("  ABC1D23  ");
        Assert.Equal("ABC1D23", placa.Valor);
    }

    [Fact]
    public void Construtor_PlacaVazia_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Placa(string.Empty));
    }

    [Fact]
    public void Construtor_PlacaNula_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Placa(null!));
    }

    [Fact]
    public void Construtor_PlacaFormatoInvalido_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Placa("AB12345"));
    }

    [Fact]
    public void Construtor_PlacaCom5Caracteres_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Placa("ABC12"));
    }

    [Fact]
    public void Construtor_PlacaCom8Caracteres_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Placa("ABC12345"));
    }

    [Fact]
    public void ConversaoImplicita_ParaString_DeveRetornarValor()
    {
        var placa = new Placa("ABC1D23");
        string valor = placa;
        Assert.Equal("ABC1D23", valor);
    }

    [Fact]
    public void ToString_DeveRetornarValorFormatado()
    {
        var placa = new Placa("ABC1D23");
        Assert.Equal("ABC1D23", placa.ToString());
    }

    [Fact]
    public void Equals_DuasPlacasComMesmoValor_DeveSerIgual()
    {
        var placa1 = new Placa("ABC1D23");
        var placa2 = new Placa("abc1d23");
        Assert.Equal(placa1, placa2);
    }

    [Fact]
    public void Equals_PlacasDiferentes_NaoDeveSerIgual()
    {
        var placa1 = new Placa("ABC1D23");
        var placa2 = new Placa("XYZ9876");
        Assert.NotEqual(placa1, placa2);
    }

    [Fact]
    public void GetHashCode_MesmoValor_DeveRetornarMesmoHash()
    {
        var placa1 = new Placa("ABC1D23");
        var placa2 = new Placa("ABC1D23");
        Assert.Equal(placa1.GetHashCode(), placa2.GetHashCode());
    }
}
