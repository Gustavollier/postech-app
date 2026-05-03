using Moq;
using PosTechChallenge.Aplicacao.Dto.ItemOS;
using PosTechChallenge.Dominio.Results;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.Integration;

public class ItensOSControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ItensOSControllerTests(CustomWebApplicationFactory factory)
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
        _factory.ItemOSServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarItemOSDto>()))
            .ReturnsAsync(Resultado.Sucesso("Item criado com sucesso."));

        var response = await _client.PostAsJsonAsync("/api/v1/ordens-servico/10/itens", CriarBody());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Criar_QuantidadeZero_DeveRetornar400()
    {
        var body = CriarBody() with { QuantidadeItem = 0 };

        var response = await _client.PostAsJsonAsync("/api/v1/ordens-servico/10/itens", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorOrdemServico_ComItens_DeveRetornar200()
    {
        _factory.ItemOSServiceMock
            .Setup(s => s.ObterPorOrdemServicoIdAsync(10))
            .ReturnsAsync(Resultado<IEnumerable<ObterItemOSDto>>.Sucesso(new[] { ObterDto() }));

        var response = await _client.GetAsync("/api/v1/ordens-servico/10/itens");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorId_NaoEncontrado_DeveRetornar404()
    {
        _factory.ItemOSServiceMock
            .Setup(s => s.ObterPorIdAsync(10, 999))
            .ReturnsAsync(Resultado<ObterItemOSDto>.Falha("Item nao encontrado."));

        var response = await _client.GetAsync("/api/v1/ordens-servico/10/itens/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Atualizar_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PutAsJsonAsync("/api/v1/ordens-servico/10/itens/1", AtualizarBody());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Deletar_GerenteValido_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CustomWebApplicationFactory.GerarToken("Gerente"));
        _factory.ItemOSServiceMock
            .Setup(s => s.DeletarAsync(10, 1))
            .ReturnsAsync(Resultado.Sucesso("Item deletado com sucesso."));

        var response = await _client.DeleteAsync("/api/v1/ordens-servico/10/itens/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static ItemOSBody CriarBody() => new(
        TipoItem: ETipoItemOrdemServico.Peca,
        QuantidadeItem: 2,
        IdFuncionario: null,
        IdPeca: 5);

    private static ItemOSBody AtualizarBody() => new(
        TipoItem: ETipoItemOrdemServico.Peca,
        QuantidadeItem: 3,
        IdFuncionario: null,
        IdPeca: 5);

    private static ObterItemOSDto ObterDto() => new()
    {
        Id = 1,
        IdOS = 10,
        TipoItem = ETipoItemOrdemServico.Peca,
        QuantidadeItem = 2,
        IdFuncionario = null,
        IdPeca = 5
    };

    private sealed record ItemOSBody(
        ETipoItemOrdemServico TipoItem,
        int QuantidadeItem,
        int? IdFuncionario,
        int? IdPeca);
}
