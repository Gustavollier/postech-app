using System.Collections.Concurrent;
using System.Text.Json;

namespace PosTechChallenge.Middleware;

public sealed class LoginRateLimitingMiddleware
{
    private const int MaxRequestsPerMinute = 10;
    private const int MaxFailedAttemptsByCpf = 5;
    private static readonly TimeSpan RequestWindow = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private readonly RequestDelegate _next;
    private readonly ILogger<LoginRateLimitingMiddleware> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly ConcurrentDictionary<string, IpRateLimitState> _ipRequests = new();
    private readonly ConcurrentDictionary<string, LoginFailureState> _cpfFailures = new();

    public LoginRateLimitingMiddleware(
        RequestDelegate next,
        ILogger<LoginRateLimitingMiddleware> logger,
        TimeProvider timeProvider)
    {
        _next = next;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsLoginRequest(context))
        {
            await _next(context);
            return;
        }

        var now = _timeProvider.GetUtcNow();
        var ip = GetClientIp(context);

        if (IsIpRateLimited(ip, now))
        {
            _logger.LogWarning("Rate limit excedido para login pelo IP {IpAddress}.", ip);
            await WriteTooManyRequestsAsync(context, "Muitas tentativas de login. Tente novamente em instantes.");
            return;
        }

        var cpf = await TryReadCpfAsync(context);
        if (!string.IsNullOrWhiteSpace(cpf) && IsCpfLocked(cpf, now))
        {
            _logger.LogWarning("Login bloqueado temporariamente para CPF {Cpf}.", cpf);
            await WriteTooManyRequestsAsync(context, "Muitas falhas de login. Tente novamente mais tarde.");
            return;
        }

        await _next(context);

        if (string.IsNullOrWhiteSpace(cpf))
            return;

        if (context.Response.StatusCode is >= 200 and < 300)
        {
            _cpfFailures.TryRemove(cpf, out _);
            return;
        }

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            RegisterCpfFailure(cpf, now);
    }

    private static bool IsLoginRequest(HttpContext context)
        => HttpMethods.IsPost(context.Request.Method)
            && context.Request.Path.Equals("/api/v1/autenticacao/login", StringComparison.OrdinalIgnoreCase);

    private bool IsIpRateLimited(string ip, DateTimeOffset now)
    {
        var state = _ipRequests.AddOrUpdate(
            ip,
            _ => new IpRateLimitState(now, 1),
            (_, current) =>
            {
                if (now - current.WindowStart >= RequestWindow)
                    return new IpRateLimitState(now, 1);

                return current with { Count = current.Count + 1 };
            });

        return state.Count > MaxRequestsPerMinute;
    }

    private bool IsCpfLocked(string cpf, DateTimeOffset now)
    {
        if (!_cpfFailures.TryGetValue(cpf, out var state))
            return false;

        if (state.LockedUntil is null)
            return false;

        if (state.LockedUntil <= now)
        {
            _cpfFailures.TryRemove(cpf, out _);
            return false;
        }

        return true;
    }

    private void RegisterCpfFailure(string cpf, DateTimeOffset now)
    {
        var state = _cpfFailures.AddOrUpdate(
            cpf,
            _ => new LoginFailureState(1, null),
            (_, current) =>
            {
                var attempts = current.FailedAttempts + 1;
                var lockedUntil = attempts >= MaxFailedAttemptsByCpf
                    ? now.Add(LockoutDuration)
                    : current.LockedUntil;

                return new LoginFailureState(attempts, lockedUntil);
            });

        if (state.LockedUntil is not null)
            _logger.LogWarning("CPF {Cpf} bloqueado ate {LockedUntil}.", cpf, state.LockedUntil);
    }

    private static string GetClientIp(HttpContext context)
        => context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private static async Task<string?> TryReadCpfAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        try
        {
            using var document = await JsonDocument.ParseAsync(context.Request.Body);
            context.Request.Body.Position = 0;

            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (string.Equals(property.Name, "cpf", StringComparison.OrdinalIgnoreCase))
                    return property.Value.GetString();
            }
        }
        catch (JsonException)
        {
            context.Request.Body.Position = 0;
            return null;
        }

        return null;
    }

    private static async Task WriteTooManyRequestsAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.Response.WriteAsJsonAsync(new { message });
    }

    private sealed record IpRateLimitState(DateTimeOffset WindowStart, int Count);

    private sealed record LoginFailureState(int FailedAttempts, DateTimeOffset? LockedUntil);
}
