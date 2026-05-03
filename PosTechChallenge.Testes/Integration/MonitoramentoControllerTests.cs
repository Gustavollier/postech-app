using Moq;
using PosTechChallenge.Monitoring;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace PosTechChallenge.Testes.Integration;

public class MonitoramentoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public MonitoramentoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ObterTempoExecucaoMedio_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/v1/monitoramento/tempo-execucao-medio");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ObterTempoExecucaoMedio_ComToken_DeveRetornar200ComResumo()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CustomWebApplicationFactory.GerarToken());
        _factory.ExecutionTimeMonitorMock
            .Setup(m => m.GetSummary())
            .Returns(new ExecutionTimeSummary(
                TotalRequests: 3,
                TotalDurationMs: 120,
                AverageDurationMs: 40,
                Endpoints: new[]
                {
                    new EndpointExecutionSummary("GET /api/v1/clientes", 3, 120, 40)
                }));

        var response = await _client.GetAsync("/api/v1/monitoramento/tempo-execucao-medio");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("totalRequisicoes", content);
        Assert.Contains("GET /api/v1/clientes", content);
    }
}
