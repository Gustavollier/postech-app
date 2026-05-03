using Moq;
using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Dominio.Results;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace PosTechChallenge.Testes.Integration;

public class AutenticacaoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    private const string CpfValido = "529.982.247-25";

    public AutenticacaoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_CredenciaisValidas_DeveRetornar200ComToken()
    {
        var tokenDto = new TokenResponseDto(
            AccessToken: "token-fake",
            RefreshToken: "refresh-fake",
            ExpiresIn: 900,
            FuncionarioId: 1,
            Cargo: "Mecanico");

        _factory.AutenticacaoServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Resultado<TokenResponseDto>.Sucesso(tokenDto));

        var body = new { CPF = CpfValido, Senha = "Senha@123" };

        var response = await _client.PostAsJsonAsync("/api/v1/autenticacao/login", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_CredenciaisInvalidas_DeveRetornar401()
    {
        _factory.AutenticacaoServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Resultado<TokenResponseDto>.Falha("CPF ou senha incorretos."));

        var body = new { CPF = CpfValido, Senha = "SenhaErrada@1" };

        var response = await _client.PostAsJsonAsync("/api/v1/autenticacao/login", body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_CpfInvalido_DeveRetornar400()
    {
        var body = new { CPF = "00000000000", Senha = "Senha@123" };

        var response = await _client.PostAsJsonAsync("/api/v1/autenticacao/login", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_CamposVazios_DeveRetornar400()
    {
        var body = new { CPF = "", Senha = "" };

        var response = await _client.PostAsJsonAsync("/api/v1/autenticacao/login", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CriarSenha_EndpointRemovido_DeveRetornar404()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.GerarToken("Gerente"));

        var body = new
        {
            CPF = CpfValido,
            Senha = "Senha@123",
            ConfirmacaoSenha = "Senha@123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/autenticacao/criar-senha", body);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AlterarSenha_TokenQualquerRole_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.GerarToken("Mecanico"));
        _factory.AutenticacaoServiceMock
            .Setup(s => s.AlterarSenhaAsync(1, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Resultado.Sucesso("Senha alterada com sucesso."));

        var body = new
        {
            SenhaAtual = "Senha@123",
            NovaSenha = "NovaSenha@123",
            ConfirmacaoSenha = "NovaSenha@123"
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/autenticacao/alterar-senha", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AlterarSenha_SemToken_DeveRetornar401()
    {
        var body = new
        {
            SenhaAtual = "Senha@123",
            NovaSenha = "NovaSenha@123",
            ConfirmacaoSenha = "NovaSenha@123"
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/autenticacao/alterar-senha", body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AlterarSenha_SenhasDivergentes_DeveRetornar400()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.GerarToken("Gerente"));
        _factory.AutenticacaoServiceMock
            .Setup(s => s.AlterarSenhaAsync(1, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Resultado.Falha("As senhas nao conferem."));

        var body = new
        {
            SenhaAtual = "Senha@123",
            NovaSenha = "NovaSenha@123",
            ConfirmacaoSenha = "OutraSenha@123"
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/autenticacao/alterar-senha", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
