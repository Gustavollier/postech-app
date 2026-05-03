using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Testes.ValueObjects;

public class CnpjValueObjectTests
{
    // CNPJ válido: 11.222.333/0001-81
    private const string CnpjValido = "11.222.333/0001-81";
    private const string CnpjValidoSomenteDigitos = "11222333000181";

    [Fact]
    public void Construtor_CnpjValidoFormatado_DeveCriarObjeto()
    {
        var cnpj = new CnpjValueObject(CnpjValido);
        Assert.Equal(CnpjValidoSomenteDigitos, cnpj.Valor);
    }

    [Fact]
    public void Construtor_CnpjValidoSomenteDigitos_DeveCriarObjeto()
    {
        var cnpj = new CnpjValueObject(CnpjValidoSomenteDigitos);
        Assert.Equal(CnpjValidoSomenteDigitos, cnpj.Valor);
    }

    [Fact]
    public void Construtor_CnpjVazio_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CnpjValueObject(string.Empty));
    }

    [Fact]
    public void Construtor_CnpjNulo_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CnpjValueObject(null!));
    }

    [Fact]
    public void Construtor_CnpjComMenosDe14Digitos_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CnpjValueObject("1234567890123"));
        Assert.Contains("14 dígitos", ex.Message);
    }

    [Fact]
    public void Construtor_CnpjComMaisDe14Digitos_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CnpjValueObject("123456789012345"));
        Assert.Contains("14 dígitos", ex.Message);
    }

    [Fact]
    public void Construtor_CnpjComTodosDigitosIguais_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CnpjValueObject("11111111111111"));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void Construtor_CnpjComDigitosVerificadoresErrados_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CnpjValueObject("11222333000100"));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void ConversaoImplicita_ParaString_DeveRetornarValor()
    {
        var cnpj = new CnpjValueObject(CnpjValido);
        string valor = cnpj;
        Assert.Equal(CnpjValidoSomenteDigitos, valor);
    }

    [Fact]
    public void ToString_DeveRetornarValorSemFormatacao()
    {
        var cnpj = new CnpjValueObject(CnpjValido);
        Assert.Equal(CnpjValidoSomenteDigitos, cnpj.ToString());
    }

    [Fact]
    public void Equals_DoisCnpjsComMesmoValor_DeveSerIgual()
    {
        var cnpj1 = new CnpjValueObject(CnpjValido);
        var cnpj2 = new CnpjValueObject(CnpjValidoSomenteDigitos);
        Assert.Equal(cnpj1, cnpj2);
    }

    [Fact]
    public void GetHashCode_MesmoValor_DeveRetornarMesmoHash()
    {
        var cnpj1 = new CnpjValueObject(CnpjValido);
        var cnpj2 = new CnpjValueObject(CnpjValidoSomenteDigitos);
        Assert.Equal(cnpj1.GetHashCode(), cnpj2.GetHashCode());
    }
}
