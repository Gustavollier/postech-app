using Microsoft.AspNetCore.Routing;
using PosTechChallenge.Monitoring;
using System.Diagnostics;

namespace PosTechChallenge.Middleware;

public sealed class RequestExecutionTimingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestExecutionTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IExecutionTimeMonitor monitor)
    {
        var shouldMonitor = !context.Request.Path.StartsWithSegments("/api/v1/monitoramento");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
        }

        if (!shouldMonitor)
        {
            return;
        }

        // Usa o template da rota como chave. Requisições sem rota casada (404, sondagens)
        // são ignoradas para não gerar chaves distintas em excesso no monitor.
        var route = (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText;
        if (string.IsNullOrWhiteSpace(route))
        {
            return;
        }

        var key = $"{context.Request.Method} {route}";

        monitor.Record(key, stopwatch.Elapsed);
    }
}