using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Aplicacao.Services;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using Xunit;

namespace PosTechChallenge.Testes.Application;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepositorio> _repositorio = new();
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _service = new ClienteService(_repositorio.Object, NullLogger<ClienteService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_CpfVazio_DevePersistirCpfComoNulo()
    {
        Cliente? clienteCriado = null;
        _repositorio
            .Setup(r => r.CriarAsync(It.IsAny<Cliente>()))
            .Callback<Cliente>(c => clienteCriado = c)
            .ReturnsAsync(1);

        var resultado = await _service.CriarAsync(new CriarClienteDto(
            NomeCompleto: "Cliente Teste",
            CPF: "",
            CNPJ: "11222333000181",
            Telefone: "11999990000",
            Email: "cliente@email.com"));

        Assert.True(resultado.IsValid);
        Assert.Null(clienteCriado?.CPF);
        Assert.Equal("11222333000181", clienteCriado?.CNPJ);
        Assert.True(clienteCriado?.Ativo);
    }

    [Fact]
    public async Task ObterPorIdAsync_Encontrado_DeveMapearClienteDto()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(Cliente());

        var resultado = await _service.ObterPorIdAsync(1);

        Assert.True(resultado.IsValid);
        Assert.Equal("Cliente Teste", resultado.Output?.NomeCompleto);
    }

    [Fact]
    public async Task ObterTodosAsync_SemClientes_DeveRetornarFalha()
    {
        _repositorio.Setup(r => r.ObterTodosAsync(1, 10)).ReturnsAsync(Array.Empty<Cliente>());
        _repositorio.Setup(r => r.ObterQuantidadeClientesAsync()).ReturnsAsync(0);

        var resultado = await _service.ObterTodosAsync(1, 10, CancellationToken.None);

        Assert.False(resultado.IsValid);
    }

    [Fact]
    public async Task ObterTodosAsync_ComClientes_DeveRetornarPaginacao()
    {
        _repositorio.Setup(r => r.ObterTodosAsync(1, 10)).ReturnsAsync(new[] { Cliente() });
        _repositorio.Setup(r => r.ObterQuantidadeClientesAsync()).ReturnsAsync(20);

        var resultado = await _service.ObterTodosAsync(1, 10, CancellationToken.None);

        Assert.True(resultado.IsValid);
        Assert.Equal(1, resultado.Output?.Page);
        Assert.Equal(10, resultado.Output?.PageSize);
        Assert.Equal(1, resultado.Output?.TotalItems);
        Assert.Equal(2, resultado.Output?.TotalPages);
    }

    [Fact]
    public async Task AtualizarAsync_ClienteNaoEncontrado_DeveRetornarFalha()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Cliente?)null);

        var resultado = await _service.AtualizarAsync(99, AtualizarDto());

        Assert.False(resultado.IsValid);
    }

    [Fact]
    public async Task DesativarAsync_RepositorioDesativa_DeveRetornarSucesso()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(Cliente());
        _repositorio.Setup(r => r.DesativarAsync(1)).ReturnsAsync(true);

        var resultado = await _service.DesativarAsync(1);

        Assert.True(resultado.IsValid);
    }

    private static AtualizarClienteDto AtualizarDto() => new(
        NomeCompleto: "Cliente Atualizado",
        CPF: "52998224725",
        CNPJ: null,
        Telefone: "11888880000",
        Email: "atualizado@email.com");

    private static Cliente Cliente() => new()
    {
        Id = 1,
        NomeCompleto = "Cliente Teste",
        CPF = "52998224725",
        Telefone = "11999990000",
        Email = "cliente@email.com",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Ativo = true
    };
}
