using Moq;
using PosTechChallenge.Aplicacao.Dto.Peca;
using PosTechChallenge.Dominio.Results;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PosTechChallenge.Testes.Integration;

public class PecasControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public PecasControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    // ── POST /api/v1/pecas ───────────────────────────────────────────────────

    [Fact]
    public async Task Criar_DadosValidos_DeveRetornar201()
    {
        _factory.PecaServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarPecaDto>()))
            .ReturnsAsync(Resultado.Sucesso("Peça criada com sucesso."));

        var body = new
        {
            Nome             = "Filtro de Óleo",
            Marca            = "Mann",
            Codigo           = "W719/30",
            Preco            = 45,
            UnidadeMedida    = 0,
            QuantidadeEstoque = 20
        };

        var response = await _client.PostAsJsonAsync("/api/v1/pecas", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Criar_EstoqueNegativo_DeveRetornar400()
    {
        var body = new
        {
            Nome             = "Filtro de Óleo",
            Preco            = 45,
            UnidadeMedida    = 0,
            QuantidadeEstoque = -1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/pecas", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Criar_QuandoServicoRetornaFalha_DeveRetornar400()
    {
        _factory.PecaServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarPecaDto>()))
            .ReturnsAsync(Resultado.Falha("Código de peça já cadastrado."));

        var body = new
        {
            Nome             = "Filtro de Óleo",
            Preco            = 45,
            UnidadeMedida    = 0,
            QuantidadeEstoque = 10
        };

        var response = await _client.PostAsJsonAsync("/api/v1/pecas", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── GET /api/v1/pecas ────────────────────────────────────────────────────

    [Fact]
    public async Task ObterTodos_DeveRetornar200()
    {
        var lista = new List<ObterPecaDto>
        {
            new() { Id = 1, Nome = "Filtro de Óleo", Preco = 45m,
                    QuantidadeEstoque = 20, CriadoEm = DateTime.UtcNow,
                    AtualizadoEm = DateTime.UtcNow, Ativo = true }
        };

        _factory.PecaServiceMock
            .Setup(s => s.ObterTodosAsync(false, It.IsAny<int>()))
            .ReturnsAsync(Resultado<IEnumerable<ObterPecaDto>>.Sucesso(lista));

        var response = await _client.GetAsync("/api/v1/pecas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterTodos_LimiteNegativo_DeveRetornar400()
    {
        var response = await _client.GetAsync("/api/v1/pecas?limiteEstoqueBaixo=-1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── PATCH /api/v1/pecas/{id}/estoque — requer Gerente ───────────────────

    [Fact]
    public async Task AjustarEstoque_SemToken_DeveRetornar401()
    {
        var body = new { Quantidade = 5 };

        var response = await _client.PatchAsJsonAsync("/api/v1/pecas/1/estoque", body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AjustarEstoque_QuantidadeZero_DeveRetornar400()
    {
        var token = CustomWebApplicationFactory.GerarToken(cargo: "Gerente");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new { Quantidade = 0 };

        var response = await _client.PatchAsJsonAsync("/api/v1/pecas/1/estoque", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task AjustarEstoque_TokenGerente_DadosValidos_DeveRetornar200()
    {
        _factory.PecaServiceMock
            .Setup(s => s.AjustarEstoqueAsync(It.IsAny<int>(), It.IsAny<AjustarEstoquePecaDto>()))
            .ReturnsAsync(Resultado.Sucesso("Estoque ajustado com sucesso."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Gerente");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new { Quantidade = 10 };

        var response = await _client.PatchAsJsonAsync("/api/v1/pecas/1/estoque", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    // ── DELETE /api/v1/pecas/{id} — requer Gerente ──────────────────────────

    [Fact]
    public async Task Desativar_SemToken_DeveRetornar401()
    {
        var response = await _client.DeleteAsync("/api/v1/pecas/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Desativar_TokenGerente_PecaExistente_DeveRetornar200()
    {
        _factory.PecaServiceMock
            .Setup(s => s.DesativarAsync(1))
            .ReturnsAsync(Resultado.Sucesso("Peça desativada com sucesso."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Gerente");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync("/api/v1/pecas/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Desativar_TokenGerente_PecaInexistente_DeveRetornar404()
    {
        _factory.PecaServiceMock
            .Setup(s => s.DesativarAsync(99))
            .ReturnsAsync(Resultado.Falha("Peça não encontrada."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Gerente");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync("/api/v1/pecas/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }
}
