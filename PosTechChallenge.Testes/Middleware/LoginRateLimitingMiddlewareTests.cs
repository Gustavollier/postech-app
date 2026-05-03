using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using PosTechChallenge.Middleware;
using Xunit;

namespace PosTechChallenge.Testes.Middleware;

public class LoginRateLimitingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_AteDezTentativasPorMinuto_DevePermitir()
    {
        var timeProvider = new ManualTimeProvider(DateTimeOffset.UtcNow);
        var chamadas = 0;
        var middleware = CriarMiddleware(_ =>
        {
            chamadas++;
            return Task.CompletedTask;
        }, timeProvider);

        for (var i = 0; i < 10; i++)
        {
            var context = CriarContexto("{}");

            await middleware.InvokeAsync(context);

            Assert.NotEqual(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
        }

        Assert.Equal(10, chamadas);
    }

    [Fact]
    public async Task InvokeAsync_DecimaPrimeiraTentativaNoMesmoMinuto_DeveRetornar429()
    {
        var timeProvider = new ManualTimeProvider(DateTimeOffset.UtcNow);
        var chamadas = 0;
        var middleware = CriarMiddleware(_ =>
        {
            chamadas++;
            return Task.CompletedTask;
        }, timeProvider);

        for (var i = 0; i < 10; i++)
            await middleware.InvokeAsync(CriarContexto("{}"));

        var bloqueada = CriarContexto("{}");
        await middleware.InvokeAsync(bloqueada);

        Assert.Equal(StatusCodes.Status429TooManyRequests, bloqueada.Response.StatusCode);
        Assert.Equal(10, chamadas);
    }

    [Fact]
    public async Task InvokeAsync_AposCincoFalhasDoMesmoCpf_DeveBloquearPorQuinzeMinutos()
    {
        var timeProvider = new ManualTimeProvider(DateTimeOffset.UtcNow);
        var chamadas = 0;
        var middleware = CriarMiddleware(context =>
        {
            chamadas++;
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }, timeProvider);

        for (var i = 0; i < 5; i++)
            await middleware.InvokeAsync(CriarContexto("""{"CPF":"52998224725"}"""));

        var bloqueada = CriarContexto("""{"CPF":"52998224725"}""");
        await middleware.InvokeAsync(bloqueada);

        Assert.Equal(StatusCodes.Status429TooManyRequests, bloqueada.Response.StatusCode);
        Assert.Equal(5, chamadas);
    }

    [Fact]
    public async Task InvokeAsync_LoginBemSucedido_DeveLimparFalhasDoCpf()
    {
        var timeProvider = new ManualTimeProvider(DateTimeOffset.UtcNow);
        var deveFalhar = true;
        var middleware = CriarMiddleware(context =>
        {
            context.Response.StatusCode = deveFalhar
                ? StatusCodes.Status401Unauthorized
                : StatusCodes.Status200OK;

            return Task.CompletedTask;
        }, timeProvider);

        for (var i = 0; i < 4; i++)
            await middleware.InvokeAsync(CriarContexto("""{"CPF":"52998224725"}"""));

        deveFalhar = false;
        await middleware.InvokeAsync(CriarContexto("""{"CPF":"52998224725"}"""));

        deveFalhar = true;
        for (var i = 0; i < 4; i++)
            await middleware.InvokeAsync(CriarContexto("""{"CPF":"52998224725"}"""));

        var aindaPermitida = CriarContexto("""{"CPF":"52998224725"}""");
        await middleware.InvokeAsync(aindaPermitida);

        Assert.Equal(StatusCodes.Status401Unauthorized, aindaPermitida.Response.StatusCode);
    }

    private static LoginRateLimitingMiddleware CriarMiddleware(
        RequestDelegate next,
        TimeProvider timeProvider)
        => new(next, NullLogger<LoginRateLimitingMiddleware>.Instance, timeProvider);

    private static DefaultHttpContext CriarContexto(string body)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/v1/autenticacao/login";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        context.Request.ContentType = "application/json";
        context.Response.Body = new MemoryStream();
        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

        return context;
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public ManualTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
