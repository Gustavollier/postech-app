using PosTechChallenge.Monitoring;
using Xunit;

namespace PosTechChallenge.Testes.Monitoring;

public class ExecutionTimeMonitorTests
{
    [Fact]
    public void GetSummary_SemRegistros_DeveRetornarResumoZerado()
    {
        var monitor = new ExecutionTimeMonitor();

        var summary = monitor.GetSummary();

        Assert.Equal(0, summary.TotalRequests);
        Assert.Equal(0, summary.TotalDurationMs);
        Assert.Equal(0, summary.AverageDurationMs);
        Assert.Empty(summary.Endpoints);
    }

    [Fact]
    public void Record_ComMultiplosEndpoints_DeveAgregarEOrdenarResumo()
    {
        var monitor = new ExecutionTimeMonitor();

        monitor.Record("GET /api/v1/clientes", TimeSpan.FromMilliseconds(30));
        monitor.Record("GET /api/v1/clientes", TimeSpan.FromMilliseconds(50));
        monitor.Record("POST /api/v1/pecas", TimeSpan.FromMilliseconds(10));

        var summary = monitor.GetSummary();

        Assert.Equal(3, summary.TotalRequests);
        Assert.Equal(90, summary.TotalDurationMs);
        Assert.Equal(30, summary.AverageDurationMs);
        Assert.Equal("GET /api/v1/clientes", summary.Endpoints.First().Endpoint);
        Assert.Equal(2, summary.Endpoints.First().TotalRequests);
        Assert.Equal(40, summary.Endpoints.First().AverageDurationMs);
    }
}
