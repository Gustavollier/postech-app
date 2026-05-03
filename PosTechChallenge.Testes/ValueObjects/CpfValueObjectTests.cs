using PosTechChallenge.Dominio.ValueObjects;
using Xunit;
namespace PosTechChallenge.Testes.ValueObjects;

public class CpfValueObjectTests
{
    // CPF válido: 529.982.247-25
    private const string CpfValido = "529.982.247-25";
    private const string CpfValidoSomenteDigitos = "52998224725";

    [Fact]
    public void Construtor_CpfValidoFormatado_DeveCriarObjeto()
    {
        var cpf = new CpfValueObject(CpfValido);
        Assert.Equal(CpfValidoSomenteDigitos, cpf.Valor);
    }

    [Fact]
    public void Construtor_CpfValidoSomenteDigitos_DeveCriarObjeto()
    {
        var cpf = new CpfValueObject(CpfValidoSomenteDigitos);
        Assert.Equal(CpfValidoSomenteDigitos, cpf.Valor);
    }

    [Fact]
    public void Construtor_CpfVazio_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CpfValueObject(string.Empty));
    }

    [Fact]
    public void Construtor_CpfNulo_DeveLancarArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CpfValueObject(null!));
    }

    [Fact]
    public void Construtor_CpfComMenosDe11Digitos_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CpfValueObject("1234567890"));
        Assert.Contains("11 dígitos", ex.Message);
    }

    [Fact]
    public void Construtor_CpfComMaisDe11Digitos_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CpfValueObject("123456789012"));
        Assert.Contains("11 dígitos", ex.Message);
    }

    [Fact]
    public void Construtor_CpfComTodosDigitosIguais_DeveLancarArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CpfValueObject("11111111111"));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void Construtor_CpfComDigitosVerificadoresErrados_DeveLancarArgumentException()
    {
        // CPF com estrutura válida mas dígitos verificadores incorretos
        var ex = Assert.Throws<ArgumentException>(() => new CpfValueObject("52998224700"));
        Assert.Contains("inválido", ex.Message);
    }

    [Fact]
    public void ConversaoImplicita_ParaString_DeveRetornarValor()
    {
        var cpf = new CpfValueObject(CpfValido);
        string valor = cpf;
        Assert.Equal(CpfValidoSomenteDigitos, valor);
    }

    [Fact]
    public void ToString_DeveRetornarValorSemFormatacao()
    {
        var cpf = new CpfValueObject(CpfValido);
        Assert.Equal(CpfValidoSomenteDigitos, cpf.ToString());
    }

    [Fact]
    public void Equals_DoisCpfsComMesmoValor_DeveSerIgual()
    {
        var cpf1 = new CpfValueObject(CpfValido);
        var cpf2 = new CpfValueObject(CpfValidoSomenteDigitos);
        Assert.Equal(cpf1, cpf2);
    }

    [Fact]
    public void Equals_DoisCpfsDiferentes_NaoDeveSerIgual()
    {
        var cpf1 = new CpfValueObject(CpfValido);
        // CPF válido diferente: 111.444.777-35
        var cpf2 = new CpfValueObject("11144477735");
        Assert.NotEqual(cpf1, cpf2);
    }

    [Fact]
    public void GetHashCode_MesmoValor_DeveRetornarMesmoHash()
    {
        var cpf1 = new CpfValueObject(CpfValido);
        var cpf2 = new CpfValueObject(CpfValidoSomenteDigitos);
        Assert.Equal(cpf1.GetHashCode(), cpf2.GetHashCode());
    }
}
