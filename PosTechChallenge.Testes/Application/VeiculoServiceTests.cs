using Moq;
using PosTechChallenge.Aplicacao.Dto.Veiculo;
using PosTechChallenge.Aplicacao.Services;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using Xunit;

namespace PosTechChallenge.Testes.Application;

public class VeiculoServiceTests
{
    private readonly Mock<IVeiculosRepositorio> _repositorio = new();
    private readonly VeiculoService _service;

    public VeiculoServiceTests()
    {
        _service = new VeiculoService(_repositorio.Object);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveUppercasePlacaECriarVeiculo()
    {
        Veiculo? veiculoCriado = null;
        _repositorio
            .Setup(r => r.CriarAsync(It.IsAny<Veiculo>()))
            .Callback<Veiculo>(v => veiculoCriado = v)
            .ReturnsAsync(1);

        var resultado = await _service.CriarAsync(CriarDto(placa: "abc1d23"));

        Assert.True(resultado.IsValid);
        Assert.Equal("ABC1D23", veiculoCriado?.Placa);
        Assert.True(veiculoCriado?.Ativo);
    }

    [Fact]
    public async Task ObterPorIdAsync_Encontrado_DeveMapearDto()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(Veiculo());

        var resultado = await _service.ObterPorIdAsync(1);

        Assert.True(resultado.IsValid);
        Assert.Equal("ABC1D23", resultado.Output?.Placa);
        Assert.Equal("Corolla", resultado.Output?.Modelo);
    }

    [Fact]
    public async Task ObterPorIdAsync_NaoEncontrado_DeveRetornarFalha()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Veiculo?)null);

        var resultado = await _service.ObterPorIdAsync(99);

        Assert.False(resultado.IsValid);
        Assert.Contains("99", resultado.Message);
    }

    [Fact]
    public async Task ObterPorClienteIdAsync_SemVeiculos_DeveRetornarFalha()
    {
        _repositorio
            .Setup(r => r.ObterPorClienteIdAsync(10))
            .ReturnsAsync(Array.Empty<Veiculo>());

        var resultado = await _service.ObterPorClienteIdAsync(10);

        Assert.False(resultado.IsValid);
    }

    [Fact]
    public async Task AtualizarAsync_EncontradoERepositorioAtualiza_DeveRetornarSucesso()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(Veiculo());
        _repositorio.Setup(r => r.AtualizarAsync(It.IsAny<Veiculo>())).ReturnsAsync(true);

        var resultado = await _service.AtualizarAsync(1, AtualizarDto());

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task AtualizarAsync_RepositorioNaoAtualiza_DeveRetornarFalha()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(Veiculo());
        _repositorio.Setup(r => r.AtualizarAsync(It.IsAny<Veiculo>())).ReturnsAsync(false);

        var resultado = await _service.AtualizarAsync(1, AtualizarDto());

        Assert.False(resultado.IsValid);
    }

    private static CriarVeiculoDto CriarDto(string placa = "ABC1D23") => new(
        ClienteId: 10,
        Marca: "Toyota",
        Modelo: "Corolla",
        Placa: placa,
        Cor: "Prata",
        AnoModelo: 2022,
        AnoFabricacao: 2021,
        KmEntrada: 42000);

    private static AtualizarVeiculoDto AtualizarDto() => new(
        ClienteId: 10,
        Marca: "Toyota",
        Modelo: "Corolla XEi",
        Placa: "ABC1D23",
        Cor: "Prata",
        AnoModelo: 2023,
        AnoFabricacao: 2022,
        KmEntrada: 43000);

    private static Veiculo Veiculo() => new()
    {
        Id = 1,
        ClienteId = 10,
        Marca = "Toyota",
        Modelo = "Corolla",
        Placa = "ABC1D23",
        Cor = "Prata",
        AnoModelo = 2022,
        AnoFabricacao = 2021,
        KmEntrada = 42000,
        Ativo = true
    };
}
