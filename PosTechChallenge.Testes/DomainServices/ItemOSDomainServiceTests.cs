using Moq;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Services;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.DomainServices;

public class ItemOSDomainServiceTests
{
    private readonly Mock<IFuncionarioRepositorio> _funcionarioRepositorioMock;
    private readonly Mock<IPecasRepositorio> _pecasRepositorioMock;
    private readonly ItemOSDomainService _service;

    public ItemOSDomainServiceTests()
    {
        _funcionarioRepositorioMock = new Mock<IFuncionarioRepositorio>();
        _pecasRepositorioMock = new Mock<IPecasRepositorio>();

        _service = new ItemOSDomainService(
            _funcionarioRepositorioMock.Object,
            _pecasRepositorioMock.Object);
    }

    // ========== ValidarInclusaoAsync - Status da OS ==========

    [Theory]
    [InlineData(EStatusOrdemServico.EmExecucao)]
    [InlineData(EStatusOrdemServico.Finalizada)]
    [InlineData(EStatusOrdemServico.Entregue)]
    public async Task ValidarInclusaoAsync_OSEmStatusImutavel_DeveRetornarFalha(EStatusOrdemServico status)
    {
        var os = CriarOrdemServico(status: status);
        var item = CriarItemMaoDeObra();

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
    }

    [Theory]
    [InlineData(EStatusOrdemServico.Recebida)]
    [InlineData(EStatusOrdemServico.EmDiagnostico)]
    [InlineData(EStatusOrdemServico.AguardandoAprovacao)]
    public async Task ValidarInclusaoAsync_OSEmStatusEditavel_ItemMaoDeObraValido_DeveRetornarSucesso(EStatusOrdemServico status)
    {
        var os = CriarOrdemServico(status: status);
        var item = CriarItemMaoDeObra(idFuncionario: 1);
        ConfigurarFuncionario(1);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.True(resultado.IsValid);
    }

    // ========== ValidarInclusaoAsync - Tipo de item ==========

    [Fact]
    public async Task ValidarInclusaoAsync_TipoItemInvalido_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = new ItemOS
        {
            IdOS = 1,
            TipoItem = (ETipoItemOrdemServico)99,
            QuantidadeItem = 1
        };

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("inválido", resultado.Message);
    }

    // ========== ValidarInclusaoAsync - Quantidade ==========

    [Fact]
    public async Task ValidarInclusaoAsync_QuantidadeZero_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemMaoDeObra(quantidade: 0);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("maior que zero", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_QuantidadeNegativa_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemMaoDeObra(quantidade: -1);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("maior que zero", resultado.Message);
    }

    // ========== ValidarInclusaoAsync - Mão de obra ==========

    [Fact]
    public async Task ValidarInclusaoAsync_MaoDeObra_SemFuncionario_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemMaoDeObra(idFuncionario: 0);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("funcionário responsável", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_MaoDeObra_ComPecaInformada_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemMaoDeObra(idFuncionario: 1, idPeca: 5);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("não devem informar peça", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_MaoDeObra_FuncionarioNaoEncontrado_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemMaoDeObra(idFuncionario: 99);
        _funcionarioRepositorioMock.Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Funcionario?)null);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("Funcionário", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_MaoDeObra_Valida_DeveRetornarSucesso()
    {
        var os = CriarOrdemServico();
        var item = CriarItemMaoDeObra(idFuncionario: 1);
        ConfigurarFuncionario(1);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.True(resultado.IsValid);
    }

    // ========== ValidarInclusaoAsync - Peça ==========

    [Fact]
    public async Task ValidarInclusaoAsync_Peca_SemIdPeca_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemPeca(idPeca: 0);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("peça informada", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_Peca_ComFuncionarioInformado_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemPeca(idPeca: 1, idFuncionario: 5);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("não devem informar funcionário", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_Peca_NaoEncontrada_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemPeca(idPeca: 99, quantidade: 1);
        _pecasRepositorioMock.Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Pecas?)null);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("não encontrada", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_Peca_Inativa_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemPeca(idPeca: 1, quantidade: 1);
        var peca = new Pecas { Id = 1, Ativo = false, QuantidadeEstoque = 10 };
        _pecasRepositorioMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(peca);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("inativa", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_Peca_EstoqueInsuficiente_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var item = CriarItemPeca(idPeca: 1, quantidade: 10);
        var peca = new Pecas { Id = 1, Ativo = true, QuantidadeEstoque = 5 };
        _pecasRepositorioMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(peca);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.False(resultado.IsValid);
        Assert.Contains("Estoque insuficiente", resultado.Message);
    }

    [Fact]
    public async Task ValidarInclusaoAsync_Peca_EstoqueSuficiente_DeveRetornarSucesso()
    {
        var os = CriarOrdemServico();
        var item = CriarItemPeca(idPeca: 1, quantidade: 3);
        var peca = new Pecas { Id = 1, Ativo = true, QuantidadeEstoque = 10 };
        _pecasRepositorioMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(peca);

        var resultado = await _service.ValidarInclusaoAsync(os, item);

        Assert.True(resultado.IsValid);
    }

    // ========== ValidarRemocao ==========

    [Theory]
    [InlineData(EStatusOrdemServico.EmExecucao)]
    [InlineData(EStatusOrdemServico.Finalizada)]
    [InlineData(EStatusOrdemServico.Entregue)]
    public void ValidarRemocao_OSEmStatusImutavel_DeveRetornarFalha(EStatusOrdemServico status)
    {
        var os = CriarOrdemServico(status: status);

        var resultado = _service.ValidarRemocao(os);

        Assert.False(resultado.IsValid);
    }

    [Theory]
    [InlineData(EStatusOrdemServico.Recebida)]
    [InlineData(EStatusOrdemServico.EmDiagnostico)]
    [InlineData(EStatusOrdemServico.AguardandoAprovacao)]
    public void ValidarRemocao_OSEmStatusEditavel_DeveRetornarSucesso(EStatusOrdemServico status)
    {
        var os = CriarOrdemServico(status: status);

        var resultado = _service.ValidarRemocao(os);

        Assert.True(resultado.IsValid);
    }

    // ========== Helpers ==========

    private static OrdemServico CriarOrdemServico(
        int id = 1,
        EStatusOrdemServico status = EStatusOrdemServico.Recebida)
    {
        return new OrdemServico
        {
            Id = id,
            IdCliente = 1,
            IdVeiculo = 1,
            IdFuncionario = 1,
            Status = status,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
    }

    private static ItemOS CriarItemMaoDeObra(
        int idFuncionario = 1,
        int idPeca = 0,
        int quantidade = 1)
    {
        return new ItemOS
        {
            IdOS = 1,
            TipoItem = ETipoItemOrdemServico.MaoDeObra,
            QuantidadeItem = quantidade,
            IdFuncionario = idFuncionario,
            IdPeca = idPeca
        };
    }

    private static ItemOS CriarItemPeca(
        int idPeca = 1,
        int idFuncionario = 0,
        int quantidade = 1)
    {
        return new ItemOS
        {
            IdOS = 1,
            TipoItem = ETipoItemOrdemServico.Peca,
            QuantidadeItem = quantidade,
            IdFuncionario = idFuncionario,
            IdPeca = idPeca
        };
    }

    private void ConfigurarFuncionario(int idFuncionario)
    {
        var funcionario = new Funcionario { Id = idFuncionario };
        _funcionarioRepositorioMock.Setup(r => r.ObterPorIdAsync(idFuncionario, It.IsAny<CancellationToken>())).ReturnsAsync(funcionario);
    }
}
