using Moq;
using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Dominio.Results;
using System.Net;
using Xunit;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PosTechChallenge.Testes.Integration;

public class ClienteControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    // CPF válido: 529.982.247-25
    private const string CpfValido = "52998224725";

    public ClienteControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.GerarToken());
    }

    // ── POST /api/v1/clientes ────────────────────────────────────────────────

    [Fact]
    public async Task Criar_ClienteComCpfValido_DeveRetornar201()
    {
        _factory.ClienteServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarClienteDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado.Sucesso("Cliente criado com sucesso."));

        var body = new { NomeCompleto = "João da Silva", CPF = CpfValido };

        var response = await _client.PostAsJsonAsync("/api/v1/clientes", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Criar_SemCpfNemCnpj_DeveRetornar400()
    {
        var body = new { NomeCompleto = "João da Silva" };

        var response = await _client.PostAsJsonAsync("/api/v1/clientes", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Criar_CpfECnpjSimultaneos_DeveRetornar400()
    {
        var body = new
        {
            NomeCompleto = "João da Silva",
            CPF  = CpfValido,
            CNPJ = "11222333000181"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/clientes", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Criar_CpfInvalido_DeveRetornar400()
    {
        var body = new { NomeCompleto = "João da Silva", CPF = "11111111111" };

        var response = await _client.PostAsJsonAsync("/api/v1/clientes", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Criar_QuandoServicoRetornaFalha_DeveRetornar400()
    {
        _factory.ClienteServiceMock
            .Setup(s => s.CriarAsync(It.IsAny<CriarClienteDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado.Falha("CPF já cadastrado."));

        var body = new { NomeCompleto = "João da Silva", CPF = CpfValido };

        var response = await _client.PostAsJsonAsync("/api/v1/clientes", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── GET /api/v1/clientes ─────────────────────────────────────────────────

    [Fact]
    public async Task ObterTodos_PaginacaoValida_DeveRetornar200()
    {
        var dto = new ObterClienteDto
        {
            Items     = [],
            Page      = 1,
            PageSize  = 10,
            TotalItems = 0,
            TotalPages = 0
        };

        _factory.ClienteServiceMock
            .Setup(s => s.ObterTodosAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado<ObterClienteDto>.Sucesso(dto));

        var response = await _client.GetAsync("/api/v1/clientes?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterTodos_PageZero_DeveRetornar400()
    {
        var response = await _client.GetAsync("/api/v1/clientes?page=0&pageSize=10");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── GET /api/v1/clientes/{id} ────────────────────────────────────────────

    [Fact]
    public async Task ObterPorId_ClienteExistente_DeveRetornar200()
    {
        var clienteDto = new ClienteDto
        {
            Id           = 1,
            NomeCompleto = "João da Silva",
            CPF          = CpfValido,
            Ativo        = true,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };

        _factory.ClienteServiceMock
            .Setup(s => s.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado<ClienteDto>.Sucesso(clienteDto));

        var response = await _client.GetAsync("/api/v1/clientes/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorId_ClienteInexistente_DeveRetornar404()
    {
        _factory.ClienteServiceMock
            .Setup(s => s.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado<ClienteDto>.Falha("Cliente não encontrado."));

        var response = await _client.GetAsync("/api/v1/clientes/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── GET /api/v1/clientes/cpf-cnpj/{cpfCnpj} ─────────────────────────────

    [Fact]
    public async Task ObterPorCpfCnpj_ClienteExistente_DeveRetornar200()
    {
        var clienteDto = new ClienteDto
        {
            Id           = 1,
            NomeCompleto = "João da Silva",
            CPF          = CpfValido,
            Ativo        = true,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };

        _factory.ClienteServiceMock
            .Setup(s => s.ObterPorCpfCnpjAsync(CpfValido, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado<ClienteDto>.Sucesso(clienteDto));

        var response = await _client.GetAsync($"/api/v1/clientes/cpf-cnpj/{CpfValido}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── PUT /api/v1/clientes/{id} — requer Gerente ───────────────────────────

    [Fact]
    public async Task Atualizar_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var body = new { NomeCompleto = "João Atualizado", CPF = CpfValido };

        var response = await _client.PutAsJsonAsync("/api/v1/clientes/1", body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Atualizar_TokenSemRoleGerente_DeveRetornar403()
    {
        var token = CustomWebApplicationFactory.GerarToken(cargo: "Mecanico");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new { NomeCompleto = "João Atualizado", CPF = CpfValido };

        var response = await _client.PutAsJsonAsync("/api/v1/clientes/1", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Atualizar_TokenGerente_ClienteExistente_DeveRetornar200()
    {
        _factory.ClienteServiceMock
            .Setup(s => s.AtualizarAsync(It.IsAny<int>(), It.IsAny<AtualizarClienteDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado.Sucesso("Cliente atualizado com sucesso."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Gerente");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new { NomeCompleto = "João Atualizado", CPF = CpfValido };

        var response = await _client.PutAsJsonAsync("/api/v1/clientes/1", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    // ── DELETE /api/v1/clientes/{id} — requer Gerente ────────────────────────

    [Fact]
    public async Task Desativar_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.DeleteAsync("/api/v1/clientes/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Desativar_TokenGerente_ClienteExistente_DeveRetornar200()
    {
        _factory.ClienteServiceMock
            .Setup(s => s.DesativarAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Resultado.Sucesso("Cliente desativado com sucesso."));

        var token = CustomWebApplicationFactory.GerarToken(cargo: "Gerente");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync("/api/v1/clientes/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }
}
