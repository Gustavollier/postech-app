using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace PosTechChallenge.Testes.Integracao;

[Collection(IntegrationTestCollection.Name)]
public sealed class FuncionarioIntegracaoTests
{
    private const string CpfValido = "52998224725";

    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public FuncionarioIntegracaoTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Criar_DeveComitarFuncionarioESegurancaNaMesmaTransacao()
    {
        await _factory.LimparBancoAsync(CpfValido);

        var body = new
        {
            Nome = "Maria Integração",
            Contato = "11999990000",
            CPF = CpfValido,
            Cargo = 0, // Mecanico
            ValorHora = 120,
            Senha = "Senha@123",
            ConfirmacaoSenha = "Senha@123"
        };

        var criar = await _client.PostAsJsonAsync("/api/v1/Funcionario", body);

        Assert.Equal(HttpStatusCode.Created, criar.StatusCode);

        // Funcionário e registro de segurança devem estar ambos gravados.
        var funcionarioId = await _factory.QuerySingleOrDefaultAsync<int?>(
            "SELECT Id FROM Funcionario WHERE CPF = @Cpf", new { Cpf = CpfValido });
        Assert.NotNull(funcionarioId);

        var qtdSeguranca = await _factory.QuerySingleOrDefaultAsync<int>(
            "SELECT COUNT(1) FROM Seguranca WHERE FuncionarioId = @Id", new { Id = funcionarioId });
        Assert.Equal(1, qtdSeguranca);
    }

    [Fact]
    public async Task ObterPorCpf_ComTokenValido_DeveRetornarFuncionarioCriado()
    {
        await _factory.LimparBancoAsync(CpfValido);

        var body = new
        {
            Nome = "João Integração",
            Contato = "11888880000",
            CPF = CpfValido,
            Cargo = 0,
            ValorHora = 100,
            Senha = "Senha@123",
            ConfirmacaoSenha = "Senha@123"
        };
        await _client.PostAsJsonAsync("/api/v1/Funcionario", body);

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Funcionario/cpf?cpf={CpfValido}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", IntegrationTestFactory.GerarToken());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("João Integração", json.GetProperty("nome").GetString());
    }
}
