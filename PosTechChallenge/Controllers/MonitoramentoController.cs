using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Autorizacao;
using PosTechChallenge.Monitoring;

namespace PosTechChallenge.Controllers;

[ApiController]
[Authorize(Policy = Perfis.Equipe)]
[Route("api/v1/monitoramento")]
public sealed class MonitoramentoController : ControllerBase
{
    private readonly IExecutionTimeMonitor _executionTimeMonitor;

    public MonitoramentoController(IExecutionTimeMonitor executionTimeMonitor)
    {
        _executionTimeMonitor = executionTimeMonitor;
    }

    [HttpGet("tempo-execucao-medio")]
    public IActionResult ObterTempoExecucaoMedio()
    {
        var summary = _executionTimeMonitor.GetSummary();

        return Ok(new
        {
            totalRequisicoes = summary.TotalRequests,
            tempoTotalMs = summary.TotalDurationMs,
            tempoMedioMs = summary.AverageDurationMs,
            rotas = summary.Endpoints.Select(endpoint => new
            {
                endpoint.Endpoint,
                totalRequisicoes = endpoint.TotalRequests,
                tempoTotalMs = endpoint.TotalDurationMs,
                tempoMedioMs = endpoint.AverageDurationMs
            })
        });
    }
}