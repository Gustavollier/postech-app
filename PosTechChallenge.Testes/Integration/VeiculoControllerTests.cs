using Moq;
using PosTechChallenge.Aplicacao.Dto.Veiculo;
using PosTechChallenge.Dominio.Results;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace PosTechChallenge.Testes.Integration;

public class VeiculoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public VeiculoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CustomWebApplicationFactory.GerarToken());
    }

    [Fact]
    public async Task Criar_DadosValidos_DeveRetornar201()
    {
        _factory.VeiculoServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarVeiculoDto>()))
            .ReturnsAsync(Resultado.Sucesso("Veiculo cadastrado com sucesso."));

        var response = await _client.PostAsJsonAsync("/api/v1/veiculos", CriarBody());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Criar_PlacaVazia_DeveRetornar400()
    {
        var body = CriarBody() with { Placa = "" };

        var response = await _client.PostAsJsonAsync("/api/v1/veiculos", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorId_Encontrado_DeveRetornar200()
    {
        _factory.VeiculoServiceMock
            .Setup(s => s.ObterPorIdAsync(1))
            .ReturnsAsync(Resultado<ObterVeiculoDto>.Sucesso(ObterDto()));

        var response = await _client.GetAsync("/api/v1/veiculos/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorId_NaoEncontrado_DeveRetornar404()
    {
        _factory.VeiculoServiceMock
            .Setup(s => s.ObterPorIdAsync(999))
            .ReturnsAsync(Resultado<ObterVeiculoDto>.Falha("Veiculo nao encontrado."));

        var response = await _client.GetAsync("/api/v1/veiculos/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorClienteId_ComVeiculos_DeveRetornar200()
    {
        _factory.VeiculoServiceMock
            .Setup(s => s.ObterPorClienteIdAsync(10))
            .ReturnsAsync(Resultado<IEnumerable<ObterVeiculoDto>>.Sucesso(new[] { ObterDto() }));

        var response = await _client.GetAsync("/api/v1/clientes/10/veiculos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Atualizar_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PutAsJsonAsync("/api/v1/veiculos/1", AtualizarBody());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Atualizar_GerenteValido_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CustomWebApplicationFactory.GerarToken("Gerente"));
        _factory.VeiculoServiceMock
            .Setup(s => s.AtualizarAsync(1, It.IsAny<AtualizarVeiculoDto>()))
            .ReturnsAsync(Resultado.Sucesso("Veiculo atualizado com sucesso."));

        var response = await _client.PutAsJsonAsync("/api/v1/veiculos/1", AtualizarBody());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static CriarVeiculoBody CriarBody() => new(
        ClienteId: 10,
        Marca: "Toyota",
        Modelo: "Corolla",
        Placa: "ABC1D23",
        Cor: "Prata",
        AnoModelo: 2022,
        AnoFabricacao: 2021,
        KmEntrada: 42000);

    private static AtualizarVeiculoBody AtualizarBody() => new(
        ClienteId: 10,
        Marca: "Toyota",
        Modelo: "Corolla XEi",
        Placa: "ABC1D23",
        Cor: "Prata",
        AnoModelo: 2023,
        AnoFabricacao: 2022,
        KmEntrada: 43000);

    private static ObterVeiculoDto ObterDto() => new()
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

    private sealed record CriarVeiculoBody(
        int ClienteId,
        string Marca,
        string Modelo,
        string Placa,
        string? Cor,
        int AnoModelo,
        int AnoFabricacao,
        int KmEntrada);

    private sealed record AtualizarVeiculoBody(
        int ClienteId,
        string Marca,
        string Modelo,
        string Placa,
        string? Cor,
        int AnoModelo,
        int AnoFabricacao,
        int KmEntrada);
}
