using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PosTechChallenge.Aplicacao.Dto.Peca;
using PosTechChallenge.Aplicacao.Services;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using Xunit;

namespace PosTechChallenge.Testes.Application;

public class PecaServiceTests
{
    private readonly Mock<IPecasRepositorio> _repositorio = new();
    private readonly PecaService _service;

    public PecaServiceTests()
    {
        _service = new PecaService(_repositorio.Object, NullLogger<PecaService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarPecaAtiva()
    {
        Pecas? pecaCriada = null;
        _repositorio
            .Setup(r => r.CriarAsync(It.IsAny<Pecas>(), It.IsAny<CancellationToken>()))
            .Callback<Pecas, CancellationToken>((p, _) => pecaCriada = p)
            .ReturnsAsync(1);

        var resultado = await _service.CriarAsync(new CriarPecaDto(
            Nome: "Filtro de oleo",
            Marca: "Mann",
            Codigo: "W719",
            Preco: 45,
            UnidadeMedida: 0,
            QuantidadeEstoque: 20));

        Assert.True(resultado.IsValid);
        Assert.True(pecaCriada?.Ativo);
        Assert.Equal("Filtro de oleo", pecaCriada?.Nome);
    }

    [Fact]
    public async Task ObterTodosAsync_ComEstoqueBaixo_DeveFiltrarItens()
    {
        _repositorio.Setup(r => r.ObterTodosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[]
        {
            Peca(id: 1, quantidade: 2),
            Peca(id: 2, quantidade: 20)
        });

        var resultado = await _service.ObterTodosAsync(estoqueBaixo: true, limiteEstoqueBaixo: 5);

        Assert.True(resultado.IsValid);
        Assert.Single(resultado.Output!);
        Assert.Equal(1, resultado.Output!.First().Id);
    }

    [Fact]
    public async Task ObterTodosAsync_SemItens_DeveRetornarFalha()
    {
        _repositorio.Setup(r => r.ObterTodosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Pecas>());

        var resultado = await _service.ObterTodosAsync(estoqueBaixo: false, limiteEstoqueBaixo: 5);

        Assert.False(resultado.IsValid);
    }

    [Fact]
    public async Task AjustarEstoqueAsync_EstoqueFicariaNegativo_DeveRetornarFalha()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(Peca(id: 1, quantidade: 3));

        var resultado = await _service.AjustarEstoqueAsync(1, new AjustarEstoquePecaDto(-5));

        Assert.False(resultado.IsValid);
        _repositorio.Verify(r => r.AjustarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AjustarEstoqueAsync_EstoqueValido_DeveAtualizar()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(Peca(id: 1, quantidade: 3));
        _repositorio.Setup(r => r.AjustarEstoqueAsync(1, 8, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var resultado = await _service.AjustarEstoqueAsync(1, new AjustarEstoquePecaDto(5));

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task DesativarAsync_PecaNaoEncontrada_DeveRetornarFalha()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Pecas?)null);

        var resultado = await _service.DesativarAsync(99);

        Assert.False(resultado.IsValid);
    }

    private static Pecas Peca(int id, int quantidade) => new()
    {
        Id = id,
        Nome = $"Peca {id}",
        Marca = "Marca",
        Codigo = $"COD{id}",
        Preco = 10,
        UnidadeMedida = 0,
        QuantidadeEstoque = quantidade,
        CriadoEm = DateTime.UtcNow,
        AtualizadoEm = DateTime.UtcNow,
        Ativo = true
    };
}
