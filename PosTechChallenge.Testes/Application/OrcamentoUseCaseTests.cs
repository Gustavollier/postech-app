using Moq;
using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Aplicacao.UseCases.Orcamento;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.Application;

public sealed class OrcamentoUseCaseTests
{
    private readonly Mock<IOrdemServicoRepositorio> _ordemServicoRepositorioMock = new();
    private readonly Mock<IItemsRepositorio> _itemsRepositorioMock = new();
    private readonly Mock<IOrcamentoRepositorio> _orcamentoRepositorioMock = new();
    private readonly Mock<IClienteRepositorio> _clienteRepositorioMock = new();
    private readonly Mock<IEmailOutboxRepositorio> _emailOutboxRepositorioMock = new();
    private readonly Mock<IStatusRepositorio> _statusRepositorioMock = new();

    [Fact]
    public async Task ObterPorOrdemServicoIdAsync_OrcamentoExistente_DeveRetornarOrcamento()
    {
        _orcamentoRepositorioMock
            .Setup(r => r.ObterPorOrdemServicoIdAsync(1))
            .ReturnsAsync(CriarOrcamento(EStatusOrcamento.Pendente));

        var useCase = new ObterOrcamentoUseCase(_orcamentoRepositorioMock.Object);

        var resultado = await useCase.ObterPorOrdemServicoIdAsync(1);

        Assert.True(resultado.IsValid);
        Assert.Equal(1, resultado.Output!.IdOS);
        Assert.Equal(100m, resultado.Output.ValorMaoDeObra);
        Assert.Equal(50m, resultado.Output.ValorPecas);
        Assert.Equal(150m, resultado.Output.ValorTotal);
        Assert.Equal((int)EStatusOrcamento.Pendente, resultado.Output.Status);
    }

    [Fact]
    public async Task ObterPorOrdemServicoIdAsync_OrcamentoAusente_DeveRetornarFalha()
    {
        _orcamentoRepositorioMock
            .Setup(r => r.ObterPorOrdemServicoIdAsync(99))
            .ReturnsAsync((Orcamento?)null);

        var useCase = new ObterOrcamentoUseCase(_orcamentoRepositorioMock.Object);

        var resultado = await useCase.ObterPorOrdemServicoIdAsync(99);

        Assert.False(resultado.IsValid);
        Assert.Equal("Orcamento da OS 99 nao encontrado.", resultado.Message);
        Assert.Null(resultado.Output);
    }

    [Fact]
    public async Task CalcularAsync_ComItens_DeveCriarOrcamentoComValoresCalculados()
    {
        _ordemServicoRepositorioMock
            .Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(CriarOS(EStatusOrdemServico.EmDiagnostico));
        _itemsRepositorioMock
            .Setup(r => r.ObterPorOrdemServicoIdAsync(1))
            .ReturnsAsync([new ItemOS { Id = 1, IdOS = 1, QuantidadeItem = 2 }]);
        _orcamentoRepositorioMock
            .Setup(r => r.CalcularValoresAsync(1))
            .ReturnsAsync(new OrcamentoValores { ValorMaoDeObra = 240m, ValorPecas = 79.80m, ValorTotal = 319.80m });
        _orcamentoRepositorioMock
            .Setup(r => r.CriarAsync(It.IsAny<Orcamento>()))
            .ReturnsAsync(10);

        var useCase = CriarCalcularUseCase();

        var resultado = await useCase.CalcularAsync(1);

        Assert.True(resultado.IsValid);
        Assert.Equal(240m, resultado.Output!.ValorMaoDeObra);
        Assert.Equal(79.80m, resultado.Output.ValorPecas);
        Assert.Equal(319.80m, resultado.Output.ValorTotal);
        Assert.Equal((int)EStatusOrcamento.Pendente, resultado.Output.Status);
    }

    [Fact]
    public async Task EnviarAsync_OSRecebida_DeveEnfileirarEmailEAvancarAteAguardandoAprovacao()
    {
        var os = CriarOS(EStatusOrdemServico.Recebida);
        var historico = new List<EStatusOrdemServico>();

        _ordemServicoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(os);
        _ordemServicoRepositorioMock.Setup(r => r.AtualizarAsync(It.IsAny<Dominio.Model.OrdemServico>())).ReturnsAsync(true);
        _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Cliente { Id = 1, Email = "cliente@teste.com" });
        _itemsRepositorioMock.Setup(r => r.ObterPorOrdemServicoIdAsync(1)).ReturnsAsync([new ItemOS { Id = 1, IdOS = 1, QuantidadeItem = 1 }]);
        _orcamentoRepositorioMock.Setup(r => r.CalcularValoresAsync(1)).ReturnsAsync(new OrcamentoValores { ValorMaoDeObra = 100m, ValorPecas = 50m, ValorTotal = 150m });
        _orcamentoRepositorioMock.Setup(r => r.CriarAsync(It.IsAny<Orcamento>())).ReturnsAsync(1);
        _emailOutboxRepositorioMock.Setup(r => r.CriarAsync(It.IsAny<EmailOutbox>())).ReturnsAsync(1);
        _statusRepositorioMock
            .Setup(r => r.CriarAsync(It.IsAny<Status>()))
            .Callback<Status>(status => historico.Add(status.StatusAtual))
            .ReturnsAsync(1);

        var useCase = new EnviarOrcamentoUseCase(
            _ordemServicoRepositorioMock.Object,
            _clienteRepositorioMock.Object,
            _emailOutboxRepositorioMock.Object,
            _statusRepositorioMock.Object,
            CriarCalcularUseCase());

        var resultado = await useCase.EnviarAsync(1);

        Assert.True(resultado.IsValid);
        Assert.Equal(EStatusOrdemServico.AguardandoAprovacao, os.Status);
        Assert.Equal([EStatusOrdemServico.EmDiagnostico, EStatusOrdemServico.AguardandoAprovacao], historico);
        _emailOutboxRepositorioMock.Verify(r => r.CriarAsync(It.Is<EmailOutbox>(e => e.Destinatario == "cliente@teste.com")), Times.Once);
    }

    [Fact]
    public async Task ResponderAsync_Aprovado_DeveMudarOSParaEmExecucaoEOrcamentoParaAprovado()
    {
        var os = CriarOS(EStatusOrdemServico.AguardandoAprovacao);
        var orcamento = CriarOrcamento(EStatusOrcamento.Pendente);

        _ordemServicoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(os);
        _ordemServicoRepositorioMock.Setup(r => r.AtualizarAsync(It.IsAny<Dominio.Model.OrdemServico>())).ReturnsAsync(true);
        _orcamentoRepositorioMock.Setup(r => r.ObterPorOrdemServicoIdAsync(1)).ReturnsAsync(orcamento);
        _orcamentoRepositorioMock.Setup(r => r.AtualizarAsync(It.IsAny<Orcamento>())).ReturnsAsync(true);
        _statusRepositorioMock.Setup(r => r.CriarAsync(It.IsAny<Status>())).ReturnsAsync(1);

        var useCase = new ResponderOrcamentoUseCase(
            _ordemServicoRepositorioMock.Object,
            _orcamentoRepositorioMock.Object,
            _statusRepositorioMock.Object);

        var resultado = await useCase.ResponderAsync(1, new ResponderOrcamentoDto(EStatusOrcamento.Aprovado));

        Assert.True(resultado.IsValid);
        Assert.Equal(EStatusOrdemServico.EmExecucao, os.Status);
        Assert.Equal(EStatusOrcamento.Aprovado, orcamento.Status);
    }

    [Fact]
    public async Task ResponderAsync_Rejeitado_DeveMudarOSParaCanceladaSemBaixaDeEstoque()
    {
        var os = CriarOS(EStatusOrdemServico.AguardandoAprovacao);
        var orcamento = CriarOrcamento(EStatusOrcamento.Pendente);

        _ordemServicoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(os);
        _ordemServicoRepositorioMock.Setup(r => r.AtualizarAsync(It.IsAny<Dominio.Model.OrdemServico>())).ReturnsAsync(true);
        _orcamentoRepositorioMock.Setup(r => r.ObterPorOrdemServicoIdAsync(1)).ReturnsAsync(orcamento);
        _orcamentoRepositorioMock.Setup(r => r.AtualizarAsync(It.IsAny<Orcamento>())).ReturnsAsync(true);
        _statusRepositorioMock.Setup(r => r.CriarAsync(It.IsAny<Status>())).ReturnsAsync(1);

        var useCase = new ResponderOrcamentoUseCase(
            _ordemServicoRepositorioMock.Object,
            _orcamentoRepositorioMock.Object,
            _statusRepositorioMock.Object);

        var resultado = await useCase.ResponderAsync(1, new ResponderOrcamentoDto(EStatusOrcamento.Rejeitado));

        Assert.True(resultado.IsValid);
        Assert.Equal(EStatusOrdemServico.Cancelada, os.Status);
        Assert.Equal(EStatusOrcamento.Rejeitado, orcamento.Status);
    }

    private CalcularOrcamentoUseCase CriarCalcularUseCase()
    {
        return new CalcularOrcamentoUseCase(
            _ordemServicoRepositorioMock.Object,
            _itemsRepositorioMock.Object,
            _orcamentoRepositorioMock.Object);
    }

    private static Dominio.Model.OrdemServico CriarOS(EStatusOrdemServico status)
    {
        return new Dominio.Model.OrdemServico
        {
            Id = 1,
            IdCliente = 1,
            IdVeiculo = 1,
            IdFuncionario = 1,
            Status = status,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
    }

    private static Orcamento CriarOrcamento(EStatusOrcamento status)
    {
        return new Orcamento
        {
            Id = 1,
            IdOS = 1,
            ValorMaoDeObra = 100m,
            ValorPecas = 50m,
            ValorTotal = 150m,
            Status = status,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
    }
}
