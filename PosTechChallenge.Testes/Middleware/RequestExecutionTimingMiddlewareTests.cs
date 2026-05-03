using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Moq;
using PosTechChallenge.Middleware;
using PosTechChallenge.Monitoring;
using Xunit;

namespace PosTechChallenge.Testes.Middleware;

public class RequestExecutionTimingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_RotaComum_DeveRegistrarTempoDeExecucao()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/v1/clientes";
        var endpoint = new RouteEndpoint(
            _ => Task.CompletedTask,
            RoutePatternFactory.Parse("/api/v1/clientes"),
            0,
            EndpointMetadataCollection.Empty,
            "Clientes");
        context.SetEndpoint(endpoint);

        var monitor = new Mock<IExecutionTimeMonitor>();
        var middleware = new RequestExecutionTimingMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, monitor.Object);

        monitor.Verify(m => m.Record(
            "GET /api/v1/clientes",
            It.Is<TimeSpan>(duration => duration >= TimeSpan.Zero)), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_RotaDeMonitoramento_NaoDeveRegistrarTempo()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/v1/monitoramento/tempo-execucao-medio";

        var monitor = new Mock<IExecutionTimeMonitor>();
        var middleware = new RequestExecutionTimingMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, monitor.Object);

        monitor.Verify(m => m.Record(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
    }
}
