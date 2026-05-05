using Moq;
using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Dominio.Results;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.Integration;

public sealed class OrcamentoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrcamentoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.GerarToken());
    }

    [Fact]
    public async Task ObterPorOrdemServicoId_SemToken_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        _factory.OrcamentoServiceMock
            .Setup(s => s.ObterPorOrdemServicoIdAsync(1))
            .ReturnsAsync(Resultado<ObterOrcamentoDto>.Sucesso(CriarOrcamentoDto()));

        var response = await _client.GetAsync("/api/v1/orcamentos/os/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Pendente", json.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task ObterPorOrdemServicoId_OrcamentoNaoEncontrado_DeveRetornar404()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        _factory.OrcamentoServiceMock
            .Setup(s => s.ObterPorOrdemServicoIdAsync(99))
            .ReturnsAsync(Resultado<ObterOrcamentoDto>.Falha("Orcamento da OS 99 nao encontrado."));

        var response = await _client.GetAsync("/api/v1/orcamentos/os/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Orcamento da OS 99 nao encontrado.", json.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Calcular_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsync("/api/v1/orcamentos/os/1/calcular", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Calcular_ComToken_DeveRetornar200()
    {
        _factory.OrcamentoServiceMock
            .Setup(s => s.CalcularAsync(1))
            .ReturnsAsync(Resultado<ObterOrcamentoDto>.Sucesso(CriarOrcamentoDto()));

        var response = await _client.PostAsync("/api/v1/orcamentos/os/1/calcular", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Pendente", json.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Enviar_ComToken_DeveRetornar200()
    {
        _factory.OrcamentoServiceMock
            .Setup(s => s.EnviarAsync(1))
            .ReturnsAsync(Resultado<ObterOrcamentoDto>.Sucesso(CriarOrcamentoDto(), "Orcamento enviado."));

        var response = await _client.PostAsync("/api/v1/orcamentos/os/1/enviar", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Pendente", json.RootElement.GetProperty("orcamento").GetProperty("status").GetString());
    }

    [Fact]
    public async Task Responder_SemToken_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        _factory.OrcamentoServiceMock
            .Setup(s => s.ResponderAsync(1, It.IsAny<ResponderOrcamentoDto>()))
            .ReturnsAsync(Resultado.Sucesso("Resposta registrada."));

        var response = await _client.PostAsJsonAsync("/api/v1/orcamentos/os/1/responder", new { Status = (int)EStatusOrcamento.Aprovado });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static ObterOrcamentoDto CriarOrcamentoDto()
    {
        return new ObterOrcamentoDto
        {
            Id = 1,
            IdOS = 1,
            ValorMaoDeObra = 100m,
            ValorPecas = 50m,
            ValorTotal = 150m,
            Status = (int)EStatusOrcamento.Pendente,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
    }
}
