using System.Diagnostics;

namespace PosTechChallenge.Middleware;

/// <summary>
/// Propaga um identificador de correlação por toda a requisição.
///
/// O APIM injeta <c>X-Correlation-ID</c> antes de encaminhar para cá; quando o
/// header não vem (chamada direta ao Service, por exemplo), geramos um. O valor
/// entra no escopo do <see cref="ILogger"/>, então aparece em todos os logs JSON
/// da requisição, e vira uma tag da <see cref="Activity"/> corrente, de modo que
/// o Datadog consegue ligar o log ao trace.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ObterOuGerar(context);

        context.Items[HeaderName] = correlationId;
        Activity.Current?.SetTag("correlation_id", correlationId);

        // Escrito antes de qualquer outro middleware produzir resposta: depois que
        // o corpo começa a ser enviado, os headers já foram descarregados.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using var escopo = _logger.BeginScope(new Dictionary<string, object>
        {
            ["correlationId"] = correlationId
        });

        await _next(context);
    }

    private static string ObterOuGerar(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var valores))
        {
            var recebido = valores.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(recebido) is false)
                return recebido;
        }

        // TraceIdentifier já é único por requisição e aparece nos traces do ASP.NET,
        // o que facilita cruzar com o APM quando o header não veio do gateway.
        return context.TraceIdentifier;
    }
}
