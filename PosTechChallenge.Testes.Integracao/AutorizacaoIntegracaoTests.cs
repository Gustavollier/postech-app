using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace PosTechChallenge.Testes.Integracao;

/// <summary>
/// Escopo do token de cliente nas rotas de leitura.
///
/// Os dois emissores de token — a API, no login de funcionário, e a função de
/// autenticação por CPF — assinam com o mesmo segredo e produzem tokens
/// igualmente válidos. Antes destas regras, isso bastava: um cliente lia o
/// cadastro de todos os outros e as ordens da oficina inteira.
///
/// Cada teste aqui cobre uma porta que estava aberta.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class AutorizacaoIntegracaoTests
{
    private const int ClienteDoToken = 3;
    private const int OutroCliente = 2;

    private readonly HttpClient _client;

    public AutorizacaoIntegracaoTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<HttpResponseMessage> ObterAsync(string rota, string? token)
    {
        var requisicao = new HttpRequestMessage(HttpMethod.Get, rota);
        if (token is not null)
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _client.SendAsync(requisicao);
    }

    private async Task<HttpResponseMessage> PostarAsync(string rota, object corpo, string? token)
    {
        var requisicao = new HttpRequestMessage(HttpMethod.Post, rota)
        {
            Content = JsonContent.Create(corpo)
        };
        if (token is not null)
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _client.SendAsync(requisicao);
    }

    [Fact]
    public async Task CriarFuncionario_ComTokenDeCliente_DeveNegar()
    {
        // A rota nao tinha atributo e caia na FallbackPolicy: bastava estar
        // autenticado. Um cliente criava um funcionario com cargo Gerente e
        // entrava como ele — escalacao que anulava todas as outras regras.
        var resposta = await PostarAsync(
            "/api/v1/Funcionario",
            new
            {
                nome = "Invasor",
                contato = "(11) 90000-0000",
                cpf = "52998224725",
                cargo = 2,
                valorHora = 1,
                senha = "Escalacao@123",
                confirmacaoSenha = "Escalacao@123"
            },
            IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/clientes")]
    [InlineData("/api/v1/clientes/cpf-cnpj/52998224725")]
    [InlineData("/api/v1/ordens-servico")]
    [InlineData("/api/v1/ordens-servico/ordenado-por-status")]
    [InlineData("/api/v1/pecas")]
    [InlineData("/api/v1/Funcionario")]
    [InlineData("/api/v1/Funcionario/1")]
    [InlineData("/api/v1/Funcionario/cpf?cpf=52998224725")]
    [InlineData("/api/v1/Funcionario/nome?nome=Carlos")]
    public async Task RotasDaOperacao_ComTokenDeCliente_DeveNegar(string rota)
    {
        var resposta = await ObterAsync(rota, IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/clientes")]
    [InlineData("/api/v1/ordens-servico")]
    public async Task RotasDaOperacao_ComTokenDeEquipe_NaoDeveNegar(string rota)
    {
        var resposta = await ObterAsync(rota, IntegrationTestFactory.GerarToken("Gerente"));

        // O conteúdo depende do estado do banco; o que importa aqui é que a
        // autorização não barrou.
        Assert.NotEqual(HttpStatusCode.Forbidden, resposta.StatusCode);
        Assert.NotEqual(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/clientes")]
    [InlineData("/api/v1/ordens-servico")]
    public async Task RotasDaOperacao_SemToken_DeveExigirAutenticacao(string rota)
    {
        var resposta = await ObterAsync(rota, token: null);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task OrdensDeOutroCliente_ComTokenDeCliente_DeveNegar()
    {
        // Trocar o id na URL era o caminho mais curto para ler a oficina alheia.
        var resposta = await ObterAsync(
            $"/api/v1/ordens-servico/cliente/{OutroCliente}",
            IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task PropriasOrdens_ComTokenDeCliente_NaoDeveNegar()
    {
        var resposta = await ObterAsync(
            $"/api/v1/ordens-servico/cliente/{ClienteDoToken}",
            IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        // 200 com ordens ou 404 sem nenhuma — ambos significam que a regra
        // deixou passar. O que não pode acontecer é 403.
        Assert.NotEqual(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task VeiculosDeOutroCliente_ComTokenDeCliente_DeveNegar()
    {
        // A listagem de veículos por cliente tinha a mesma brecha das ordens:
        // bastava trocar o id na URL para ver a frota alheia.
        var resposta = await ObterAsync(
            $"/api/v1/clientes/{OutroCliente}/veiculos",
            IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task PropriosVeiculos_ComTokenDeCliente_NaoDeveNegar()
    {
        var resposta = await ObterAsync(
            $"/api/v1/clientes/{ClienteDoToken}/veiculos",
            IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        Assert.NotEqual(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task CadastroDeOutroCliente_ComTokenDeCliente_DeveNegar()
    {
        var resposta = await ObterAsync(
            $"/api/v1/clientes/{OutroCliente}",
            IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task ProprioCadastro_ComTokenDeCliente_NaoDeveNegar()
    {
        var resposta = await ObterAsync(
            $"/api/v1/clientes/{ClienteDoToken}",
            IntegrationTestFactory.GerarTokenCliente(ClienteDoToken));

        Assert.NotEqual(HttpStatusCode.Forbidden, resposta.StatusCode);
    }
}
