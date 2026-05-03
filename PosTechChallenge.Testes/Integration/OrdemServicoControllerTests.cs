using Moq;
using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Dominio.Results;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.Integration;

public class OrdemServicoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrdemServicoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.GerarToken());
    }

    // ── POST /api/v1/ordens-servico ──────────────────────────────────────────

    [Fact]
    public async Task Criar_DadosValidos_DeveRetornar201()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarOrdemServicoDto>()))
            .ReturnsAsync(Resultado.Sucesso("Ordem de serviço criada com sucesso."));

        var body = new { IdCliente = 1, IdVeiculo = 1, IdFuncionario = 1 };

        var response = await _client.PostAsJsonAsync("/api/v1/ordens-servico", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Criar_QuandoServicoRetornaFalha_DeveRetornar400()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarOrdemServicoDto>()))
            .ReturnsAsync(Resultado.Falha("Cliente não encontrado."));

        var body = new { IdCliente = 99, IdVeiculo = 1, IdFuncionario = 1 };

        var response = await _client.PostAsJsonAsync("/api/v1/ordens-servico", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── GET /api/v1/ordens-servico ───────────────────────────────────────────

    [Fact]
    public async Task ObterTodos_SemFiltro_DeveRetornar200()
    {
        var lista = new List<ObterOrdemServicoDto>
        {
            new() { Id = 1, IdCliente = 1, IdVeiculo = 1, IdFuncionario = 1,
                    Status = 0, CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow }
        };

        _factory.OrdemServicoServiceMock
            .Setup(s => s.ObterTodosAsync(null, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Resultado<IEnumerable<ObterOrdemServicoDto>>.Sucesso(lista));

        var response = await _client.GetAsync("/api/v1/ordens-servico");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterTodos_FiltrandoPorStatus_DeveRetornar200()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.ObterTodosAsync(EStatusOrdemServico.Recebida, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Resultado<IEnumerable<ObterOrdemServicoDto>>.Sucesso([]));

        var response = await _client.GetAsync("/api/v1/ordens-servico?status=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── GET /api/v1/ordens-servico/{id} ─────────────────────────────────────

    [Fact]
    public async Task ObterPorId_OSExistente_DeveRetornar200()
    {
        var osDto = new ObterOrdemServicoDto
        {
            Id = 1, IdCliente = 1, IdVeiculo = 1, IdFuncionario = 1,
            Status = 0, CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };

        _factory.OrdemServicoServiceMock
            .Setup(s => s.ObterPorIdAsync(1))
            .ReturnsAsync(Resultado<ObterOrdemServicoDto>.Sucesso(osDto));

        var response = await _client.GetAsync("/api/v1/ordens-servico/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorId_OSInexistente_DeveRetornar404()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.ObterPorIdAsync(99))
            .ReturnsAsync(Resultado<ObterOrdemServicoDto>.Falha("OS não encontrada."));

        var response = await _client.GetAsync("/api/v1/ordens-servico/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── GET /api/v1/ordens-servico/{id}/status — endpoint PÚBLICO ───────────

    [Fact]
    public async Task ObterStatus_SemToken_OSExistente_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var statusDto = new ObterStatusOrdemServicoDto
        {
            OrdemServicoId = 1,
            StatusAtual    = 0,
            AtualizadoEm   = DateTime.UtcNow,
            Historico      = []
        };

        _factory.OrdemServicoServiceMock
            .Setup(s => s.ObterStatusAsync(1))
            .ReturnsAsync(Resultado<ObterStatusOrdemServicoDto>.Sucesso(statusDto));

        // Sem token — endpoint deve ser acessível publicamente
        var response = await _client.GetAsync("/api/v1/ordens-servico/1/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterStatus_OSInexistente_DeveRetornar404()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.ObterStatusAsync(99))
            .ReturnsAsync(Resultado<ObterStatusOrdemServicoDto>.Falha("OS não encontrada."));

        var response = await _client.GetAsync("/api/v1/ordens-servico/99/status");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── GET /api/v1/ordens-servico/valor/{id} ────────────────────────────────

    [Fact]
    public async Task ObterValor_OSExistente_DeveRetornar200()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.ObterValorPorIdAsync(1))
            .ReturnsAsync(Resultado<decimal>.Sucesso(350.00m));

        var response = await _client.GetAsync("/api/v1/ordens-servico/valor/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── PATCH /api/v1/ordens-servico/{id}/status — requer qualquer token ─────

    [Fact]
    public async Task AtualizarStatus_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var body = new { IdFuncionario = 1, Status = 1 };

        var response = await _client.PatchAsJsonAsync("/api/v1/ordens-servico/1/status", body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AtualizarStatus_ComToken_TransicaoValida_DeveRetornar200()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.AtualizarStatusAsync(It.IsAny<int>(), It.IsAny<AtualizarStatusOrdemServicoDto>()))
            .ReturnsAsync(Resultado.Sucesso("Status atualizado com sucesso."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Mecanico");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new { IdFuncionario = 1, Status = (int)EStatusOrdemServico.EmDiagnostico };

        var response = await _client.PatchAsJsonAsync("/api/v1/ordens-servico/1/status", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task AtualizarStatus_ComToken_TransicaoInvalida_DeveRetornar400()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.AtualizarStatusAsync(It.IsAny<int>(), It.IsAny<AtualizarStatusOrdemServicoDto>()))
            .ReturnsAsync(Resultado.Falha("Transição de status inválida."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Mecanico");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new { IdFuncionario = 1, Status = (int)EStatusOrdemServico.Entregue };

        var response = await _client.PatchAsJsonAsync("/api/v1/ordens-servico/1/status", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    // ── DELETE /api/v1/ordens-servico/{id} — requer Gerente ─────────────────

    [Fact]
    public async Task Deletar_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.DeleteAsync("/api/v1/ordens-servico/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Deletar_TokenGerente_OSExistente_DeveRetornar200()
    {
        _factory.OrdemServicoServiceMock
            .Setup(s => s.DeletarAsync(1))
            .ReturnsAsync(Resultado.Sucesso("OS removida com sucesso."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Gerente");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync("/api/v1/ordens-servico/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }
}
