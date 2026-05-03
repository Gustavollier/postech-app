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

        var endpoint = context.GetEndpoint();
        var routePattern = endpoint as RouteEndpoint;
        var route = routePattern?.RoutePattern.RawText ?? endpoint?.DisplayName;
        var method = context.Request.Method;
        var key = string.IsNullOrWhiteSpace(route) ? $"{method} {context.Request.Path}" : $"{method} {route}";

        monitor.Record(key, stopwatch.Elapsed);
    }
}