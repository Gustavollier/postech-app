using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace PosTechChallenge.Testes.Integracao;

[Collection(IntegrationTestCollection.Name)]
public sealed class ClienteIntegracaoTests
{
    private const string CpfValido = "52998224725";

    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public ClienteIntegracaoTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Criar_e_ObterPorCpfCnpj_DevePersistirEComitarCliente()
    {
        await _factory.LimparBancoAsync(CpfValido);
        var token = IntegrationTestFactory.GerarToken("Gerente");

        var body = new
        {
            NomeCompleto = "Cliente Integração",
            CPF = CpfValido,
            Telefone = "11999990000",
            Email = "cliente@teste.com"
        };

        using var criarRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/clientes")
        {
            Content = JsonContent.Create(body)
        };
        criarRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var criar = await _client.SendAsync(criarRequest);
        Assert.Equal(HttpStatusCode.Created, criar.StatusCode);

        // O cliente deve estar disponível logo após a criação.
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/clientes/cpf-cnpj/{CpfValido}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var get = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var json = await get.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Cliente Integração", json.GetProperty("nomeCompleto").GetString());

        var totalNoBanco = await _factory.QuerySingleOrDefaultAsync<int>(
            "SELECT COUNT(1) FROM Cliente WHERE CPF = @Cpf AND Ativo = 1", new { Cpf = CpfValido });
        Assert.Equal(1, totalNoBanco);
    }

    [Fact]
    public async Task ObterPorId_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/v1/clientes/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
