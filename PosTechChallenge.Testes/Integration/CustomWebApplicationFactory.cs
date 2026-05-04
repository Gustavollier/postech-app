using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Monitoring;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PosTechChallenge.Testes.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string JwtSecret = "estaEhUmaChaveSuper-SeguraComMaisDeTrintaCaracteresAlatorios!@#$%^&*()";

    public Mock<IAutenticacaoService> AutenticacaoServiceMock { get; } = new();
    public Mock<IClienteService> ClienteServiceMock { get; } = new();
    public Mock<IOrdemServicoService> OrdemServicoServiceMock { get; } = new();
    public Mock<IOrcamentoService> OrcamentoServiceMock { get; } = new();
    public Mock<IPecaService> PecaServiceMock { get; } = new();
    public Mock<IVeiculoService> VeiculoServiceMock { get; } = new();
    public Mock<IFuncionarioService> FuncionarioServiceMock { get; } = new();
    public Mock<IItemOSService> ItemOSServiceMock { get; } = new();
    public Mock<IExecutionTimeMonitor> ExecutionTimeMonitorMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            "Server=localhost;Database=TestDb;User Id=sa;Password=fake;TrustServerCertificate=True;");
        builder.UseSetting("Jwt:SecretKey", JwtSecret);
        builder.UseSetting("Jwt:Issuer", "PosTechChallenge");
        builder.UseSetting("Jwt:Audience", "PosTechChallenge-API");
        builder.UseSetting("Jwt:AccessTokenExpirationMinutes", "15");

        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IAutenticacaoService>();
            services.AddScoped(_ => AutenticacaoServiceMock.Object);

            services.RemoveAll<IClienteService>();
            services.AddScoped(_ => ClienteServiceMock.Object);

            services.RemoveAll<IOrdemServicoService>();
            services.AddScoped(_ => OrdemServicoServiceMock.Object);

            services.RemoveAll<IOrcamentoService>();
            services.AddScoped(_ => OrcamentoServiceMock.Object);

            services.RemoveAll<IPecaService>();
            services.AddScoped(_ => PecaServiceMock.Object);

            services.RemoveAll<IVeiculoService>();
            services.AddScoped(_ => VeiculoServiceMock.Object);

            services.RemoveAll<IFuncionarioService>();
            services.AddScoped(_ => FuncionarioServiceMock.Object);

            services.RemoveAll<IItemOSService>();
            services.AddScoped(_ => ItemOSServiceMock.Object);

            services.RemoveAll<IExecutionTimeMonitor>();
            services.AddSingleton(_ => ExecutionTimeMonitorMock.Object);
        });
    }

    public static string GerarToken(string cargo = "Mecanico", int funcionarioId = 1)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, funcionarioId.ToString()),
            new Claim(ClaimTypes.Role, cargo),
        };

        var token = new JwtSecurityToken(
            issuer: "PosTechChallenge",
            audience: "PosTechChallenge-API",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
