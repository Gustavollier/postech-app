using Microsoft.Extensions.Configuration;
using PosTechChallenge.Aplicacao.Services;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace PosTechChallenge.Testes.Application;

public class TokenServiceTests
{
    [Fact]
    public void GerarToken_ConfigValida_DeveRetornarJwtComClaimsEsperadas()
    {
        var service = new TokenService(CriarConfiguracao());

        var response = service.GerarToken(7, "Gerente");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);

        Assert.Equal(900, response.ExpiresIn);
        Assert.Equal(7, response.FuncionarioId);
        Assert.Equal("Gerente", response.Cargo);
        Assert.Contains(token.Claims, c => c.Type == "nameid" && c.Value == "7");
        Assert.Contains(token.Claims, c => c.Type == "role" && c.Value == "Gerente");
        Assert.Contains(token.Claims, c => c.Type == "FuncionarioId" && c.Value == "7");
    }

    [Fact]
    public void GerarToken_ExpiracaoInvalida_DeveUsarPadraoDe15Minutos()
    {
        var config = CriarConfiguracao(expirationMinutes: "valor-invalido");
        var service = new TokenService(config);

        var response = service.GerarToken(1, "Mecanico");

        Assert.Equal(900, response.ExpiresIn);
    }

    [Fact]
    public void Construtor_SemSecretKey_DeveLancarInvalidOperationException()
    {
        var config = CriarConfiguracao(secretKey: null);

        Assert.Throws<InvalidOperationException>(() => new TokenService(config));
    }

    private static IConfiguration CriarConfiguracao(
        string? secretKey = "estaEhUmaChaveSuper-SeguraComMaisDeTrintaCaracteresAlatorios!@#$%^&*()",
        string issuer = "PosTechChallenge",
        string audience = "PosTechChallenge-API",
        string expirationMinutes = "15")
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:SecretKey"] = secretKey,
            ["Jwt:Issuer"] = issuer,
            ["Jwt:Audience"] = audience,
            ["Jwt:AccessTokenExpirationMinutes"] = expirationMinutes
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
