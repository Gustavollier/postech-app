using Moq;
using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Results;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.Integration;

public class FuncionarioControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string CpfValido = "52998224725";

    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public FuncionarioControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CustomWebApplicationFactory.GerarToken("Gerente"));
    }

    [Fact]
    public async Task Criar_DadosValidos_DeveRetornar201()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        _factory.FuncionarioServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarFuncionarioDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado.Sucesso("Funcionario criado com sucesso."));

        var response = await _client.PostAsJsonAsync("/api/v1/Funcionario", CriarBody());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Criar_CpfInvalido_DeveRetornar400()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var body = CriarBody() with { CPF = "11111111111" };

        var response = await _client.PostAsJsonAsync("/api/v1/Funcionario", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ObterTodos_ComFuncionarios_DeveRetornar200()
    {
        _factory.FuncionarioServiceMock
            .Setup(s => s.ObterTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado<IEnumerable<ObterFuncionarioDto>>.Sucesso(new[] { ObterDto() }));

        var response = await _client.GetAsync("/api/v1/Funcionario");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Mecanico", json.RootElement[0].GetProperty("cargo").GetString());
    }

    [Fact]
    public async Task ObterPorCpf_NaoEncontrado_DeveRetornar404()
    {
        _factory.FuncionarioServiceMock
            .Setup(s => s.ObterPorCpfAsync(CpfValido, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado<ObterFuncionarioDto>.Falha("Funcionario nao encontrado."));

        var response = await _client.GetAsync($"/api/v1/Funcionario/cpf?cpf={CpfValido}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Atualizar_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PutAsJsonAsync($"/api/v1/Funcionario?cpf={CpfValido}", AtualizarBody());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Deletar_GerenteValido_DeveRetornar200()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CustomWebApplicationFactory.GerarToken("Gerente"));
        _factory.FuncionarioServiceMock
            .Setup(s => s.DeletarAsync(CpfValido, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado.Sucesso("Funcionario deletado com sucesso."));

        var response = await _client.DeleteAsync($"/api/v1/Funcionario?cpf={CpfValido}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static CriarFuncionarioBody CriarBody() => new(
        Nome: "Maria Silva",
        Contato: "11999990000",
        CPF: CpfValido,
        Cargo: ECargoFuncionario.Mecanico,
        ValorHora: 120,
        Senha: "Senha@123",
        ConfirmacaoSenha: "Senha@123");

    private static AtualizarFuncionarioBody AtualizarBody() => new(
        Nome: "Maria Silva",
        Contato: "11999990000",
        Cargo: ECargoFuncionario.Supervisor,
        ValorHora: 150);

    private static ObterFuncionarioDto ObterDto() => new()
    {
        Id = 1,
        Nome = "Maria Silva",
        Contato = "11999990000",
        CPF = CpfValido,
        Cargo = ECargoFuncionario.Mecanico,
        ValorHora = 120
    };

    private sealed record CriarFuncionarioBody(
        string Nome,
        string Contato,
        string CPF,
        ECargoFuncionario Cargo,
        int ValorHora,
        string Senha,
        string ConfirmacaoSenha);

    private sealed record AtualizarFuncionarioBody(
        string Nome,
        string Contato,
        ECargoFuncionario Cargo,
        int ValorHora);
}
