using Moq;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Services;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.DomainServices;

public class OrdemServicoDomainServiceTests
{
    private readonly Mock<IClienteRepositorio> _clienteRepositorioMock;
    private readonly Mock<IVeiculosRepositorio> _veiculosRepositorioMock;
    private readonly Mock<IFuncionarioRepositorio> _funcionarioRepositorioMock;
    private readonly Mock<IItemsRepositorio> _itemsRepositorioMock;
    private readonly Mock<IPecasRepositorio> _pecasRepositorioMock;
    private readonly OrdemServicoDomainService _service;

    public OrdemServicoDomainServiceTests()
    {
        _clienteRepositorioMock = new Mock<IClienteRepositorio>();
        _veiculosRepositorioMock = new Mock<IVeiculosRepositorio>();
        _funcionarioRepositorioMock = new Mock<IFuncionarioRepositorio>();
        _itemsRepositorioMock = new Mock<IItemsRepositorio>();
        _pecasRepositorioMock = new Mock<IPecasRepositorio>();

        _service = new OrdemServicoDomainService(
            _clienteRepositorioMock.Object,
            _veiculosRepositorioMock.Object,
            _funcionarioRepositorioMock.Object,
            _itemsRepositorioMock.Object,
            _pecasRepositorioMock.Object);
    }

    // ========== ValidarCriacaoAsync ==========

    [Fact]
    public async Task ValidarCriacaoAsync_StatusDiferenteDeRecebida_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.EmDiagnostico);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("Recebida", resultado.Message);
    }

    [Fact]
    public async Task ValidarCriacaoAsync_ClienteNaoEncontrado_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(os.IdCliente)).ReturnsAsync((Cliente?)null);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("Cliente", resultado.Message);
    }

    [Fact]
    public async Task ValidarCriacaoAsync_ClienteInativo_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        var cliente = new Cliente { Id = os.IdCliente, Ativo = false };
        _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(os.IdCliente)).ReturnsAsync(cliente);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("inativo", resultado.Message);
    }

    [Fact]
    public async Task ValidarCriacaoAsync_VeiculoNaoEncontrado_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        ConfigurarClienteAtivo(os.IdCliente);
        _veiculosRepositorioMock.Setup(r => r.ObterPorIdAsync(os.IdVeiculo)).ReturnsAsync((Veiculo?)null);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("Veículo", resultado.Message);
    }

    [Fact]
    public async Task ValidarCriacaoAsync_VeiculoInativo_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        ConfigurarClienteAtivo(os.IdCliente);
        var veiculo = new Veiculo { Id = os.IdVeiculo, ClienteId = os.IdCliente, Ativo = false };
        _veiculosRepositorioMock.Setup(r => r.ObterPorIdAsync(os.IdVeiculo)).ReturnsAsync(veiculo);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("inativo", resultado.Message);
    }

    [Fact]
    public async Task ValidarCriacaoAsync_VeiculoNaoPertenceAoCliente_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        ConfigurarClienteAtivo(os.IdCliente);
        var veiculo = new Veiculo { Id = os.IdVeiculo, ClienteId = 999, Ativo = true };
        _veiculosRepositorioMock.Setup(r => r.ObterPorIdAsync(os.IdVeiculo)).ReturnsAsync(veiculo);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("não pertence", resultado.Message);
    }

    [Fact]
    public async Task ValidarCriacaoAsync_FuncionarioNaoEncontrado_DeveRetornarFalha()
    {
        var os = CriarOrdemServico();
        ConfigurarClienteAtivo(os.IdCliente);
        ConfigurarVeiculoAtivo(os.IdVeiculo, os.IdCliente);
        _funcionarioRepositorioMock.Setup(r => r.ObterPorIdAsync(os.IdFuncionario)).ReturnsAsync((Funcionario?)null);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("Funcionário", resultado.Message);
    }

    [Fact]
    public async Task ValidarCriacaoAsync_DadosValidos_DeveRetornarSucesso()
    {
        var os = CriarOrdemServico();
        ConfigurarClienteAtivo(os.IdCliente);
        ConfigurarVeiculoAtivo(os.IdVeiculo, os.IdCliente);
        ConfigurarFuncionario(os.IdFuncionario);

        var resultado = await _service.ValidarCriacaoAsync(os);

        Assert.True(resultado.IsValid);
    }

    // ========== ValidarAtualizacaoAsync ==========

    [Fact]
    public async Task ValidarAtualizacaoAsync_OSEmExecucao_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.EmExecucao);

        var resultado = await _service.ValidarAtualizacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("execução", resultado.Message);
    }

    [Fact]
    public async Task ValidarAtualizacaoAsync_OSFinalizada_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.Finalizada);

        var resultado = await _service.ValidarAtualizacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("finalizada", resultado.Message);
    }

    [Fact]
    public async Task ValidarAtualizacaoAsync_OSEntregue_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.Entregue);

        var resultado = await _service.ValidarAtualizacaoAsync(os);

        Assert.False(resultado.IsValid);
        Assert.Contains("entregue", resultado.Message);
    }

    [Fact]
    public async Task ValidarAtualizacaoAsync_OSRecebida_DadosValidos_DeveRetornarSucesso()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.Recebida);
        ConfigurarClienteAtivo(os.IdCliente);
        ConfigurarVeiculoAtivo(os.IdVeiculo, os.IdCliente);
        ConfigurarFuncionario(os.IdFuncionario);

        var resultado = await _service.ValidarAtualizacaoAsync(os);

        Assert.True(resultado.IsValid);
    }

    // ========== ValidarTransicaoStatusAsync ==========

    [Fact]
    public async Task ValidarTransicaoStatusAsync_FuncionarioNaoEncontrado_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.Recebida);
        _funcionarioRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Funcionario?)null);

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.EmDiagnostico, 1);

        Assert.False(resultado.IsValid);
        Assert.Contains("Funcionário", resultado.Message);
    }

    [Fact]
    public async Task ValidarTransicaoStatusAsync_MesmoStatus_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.Recebida);
        ConfigurarFuncionario(1);

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.Recebida, 1);

        Assert.False(resultado.IsValid);
        Assert.Contains("já está", resultado.Message);
    }

    [Fact]
    public async Task ValidarTransicaoStatusAsync_TransicaoInvalida_DeveRetornarFalha()
    {
        // Tentando pular de Recebida direto para EmExecucao
        var os = CriarOrdemServico(status: EStatusOrdemServico.Recebida);
        ConfigurarFuncionario(1);

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.EmExecucao, 1);

        Assert.False(resultado.IsValid);
        Assert.Contains("Transição inválida", resultado.Message);
    }

    [Fact]
    public async Task ValidarTransicaoStatusAsync_DeRecebidaParaEmDiagnostico_DeveRetornarSucesso()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.Recebida);
        ConfigurarFuncionario(1);

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.EmDiagnostico, 1);

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task ValidarTransicaoStatusAsync_DeEmDiagnosticoParaAguardandoAprovacao_SemItens_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.EmDiagnostico);
        ConfigurarFuncionario(1);
        _itemsRepositorioMock
            .Setup(r => r.ObterPorOrdemServicoIdAsync(os.Id))
            .ReturnsAsync(Enumerable.Empty<ItemOS>());

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.AguardandoAprovacao, 1);

        Assert.False(resultado.IsValid);
        Assert.Contains("ao menos um item", resultado.Message);
    }

    [Fact]
    public async Task ValidarTransicaoStatusAsync_DeEmDiagnosticoParaAguardandoAprovacao_ComItens_DeveRetornarSucesso()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.EmDiagnostico);
        ConfigurarFuncionario(1);
        _itemsRepositorioMock
            .Setup(r => r.ObterPorOrdemServicoIdAsync(os.Id))
            .ReturnsAsync(new List<ItemOS> { new ItemOS { Id = 1, IdOS = os.Id } });

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.AguardandoAprovacao, 1);

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task ValidarTransicaoStatusAsync_DeFinalizadaParaEntregue_DeveRetornarSucesso()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.Finalizada);
        ConfigurarFuncionario(1);
        _itemsRepositorioMock
            .Setup(r => r.ObterPorOrdemServicoIdAsync(os.Id))
            .ReturnsAsync(new List<ItemOS> { new ItemOS { Id = 1, IdOS = os.Id, IdPeca = 0 } });

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.Entregue, 1);

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task ValidarTransicaoStatusAsync_FinalizandoComEstoqueInsuficiente_DeveRetornarFalha()
    {
        var os = CriarOrdemServico(status: EStatusOrdemServico.EmExecucao);
        ConfigurarFuncionario(1);

        var item = new ItemOS { Id = 1, IdOS = os.Id, IdPeca = 10, QuantidadeItem = 5 };
        _itemsRepositorioMock
            .Setup(r => r.ObterPorOrdemServicoIdAsync(os.Id))
            .ReturnsAsync(new List<ItemOS> { item });

        var peca = new Pecas { Id = 10, Ativo = true, QuantidadeEstoque = 2 };
        _pecasRepositorioMock.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(peca);

        var resultado = await _service.ValidarTransicaoStatusAsync(os, EStatusOrdemServico.Finalizada, 1);

        Assert.False(resultado.IsValid);
        Assert.Contains("Estoque insuficiente", resultado.Message);
    }

    // ========== Helpers ==========

    private static OrdemServico CriarOrdemServico(
        int id = 1,
        int idCliente = 1,
        int idVeiculo = 1,
        int idFuncionario = 1,
        EStatusOrdemServico status = EStatusOrdemServico.Recebida)
    {
        return new OrdemServico
        {
            Id = id,
            IdCliente = idCliente,
            IdVeiculo = idVeiculo,
            IdFuncionario = idFuncionario,
            Status = status,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
    }

    private void ConfigurarClienteAtivo(int idCliente)
    {
        var cliente = new Cliente { Id = idCliente, Ativo = true };
        _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(idCliente)).ReturnsAsync(cliente);
    }

    private void ConfigurarVeiculoAtivo(int idVeiculo, int idCliente)
    {
        var veiculo = new Veiculo { Id = idVeiculo, ClienteId = idCliente, Ativo = true };
        _veiculosRepositorioMock.Setup(r => r.ObterPorIdAsync(idVeiculo)).ReturnsAsync(veiculo);
    }

    private void ConfigurarFuncionario(int idFuncionario)
    {
        var funcionario = new Funcionario { Id = idFuncionario };
        _funcionarioRepositorioMock.Setup(r => r.ObterPorIdAsync(idFuncionario)).ReturnsAsync(funcionario);
    }
}
