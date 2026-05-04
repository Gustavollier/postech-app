using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.UseCases.Orcamento;

public sealed class EnviarOrcamentoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IEmailOutboxRepositorio _emailOutboxRepositorio;
    private readonly IStatusRepositorio _statusRepositorio;
    private readonly CalcularOrcamentoUseCase _calcularOrcamentoUseCase;

    public EnviarOrcamentoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        IClienteRepositorio clienteRepositorio,
        IEmailOutboxRepositorio emailOutboxRepositorio,
        IStatusRepositorio statusRepositorio,
        CalcularOrcamentoUseCase calcularOrcamentoUseCase)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _emailOutboxRepositorio = emailOutboxRepositorio;
        _statusRepositorio = statusRepositorio;
        _calcularOrcamentoUseCase = calcularOrcamentoUseCase;
    }

    public async Task<Resultado<ObterOrcamentoDto>> EnviarAsync(int ordemServicoId)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                return Resultado<ObterOrcamentoDto>.Falha($"Ordem de servico com ID {ordemServicoId} nao encontrada.");

            if (ordemServico.Status is not (EStatusOrdemServico.Recebida or EStatusOrdemServico.EmDiagnostico or EStatusOrdemServico.AguardandoAprovacao))
                return Resultado<ObterOrcamentoDto>.Falha("Orcamento so pode ser enviado para OS recebida, em diagnostico ou aguardando aprovacao.");

            var cliente = await _clienteRepositorio.ObterPorIdAsync(ordemServico.IdCliente);
            if (cliente == null)
                return Resultado<ObterOrcamentoDto>.Falha($"Cliente com ID {ordemServico.IdCliente} nao encontrado.");

            if (string.IsNullOrWhiteSpace(cliente.Email))
                return Resultado<ObterOrcamentoDto>.Falha("Cliente nao possui email cadastrado para envio do orcamento.");

            var calculo = await _calcularOrcamentoUseCase.CalcularOuAtualizarAsync(ordemServicoId);
            if (!calculo.IsValid)
                return Resultado<ObterOrcamentoDto>.Falha(calculo.Message!);

            await CriarEmailOutboxAsync(cliente.Email!, calculo.Output!);
            var statusAtualizado = await AtualizarStatusParaAguardandoAprovacaoAsync(ordemServico);
            if (!statusAtualizado.IsValid)
                return Resultado<ObterOrcamentoDto>.Falha(statusAtualizado.Message!);

            return Resultado<ObterOrcamentoDto>.Sucesso(MapearParaDto(calculo.Output!), "Orcamento enviado para a outbox de email com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado<ObterOrcamentoDto>.Falha(ex.Message);
        }
    }

    private async Task CriarEmailOutboxAsync(string destinatario, Dominio.Model.Orcamento orcamento)
    {
        var email = new EmailOutbox
        {
            Destinatario = destinatario,
            Assunto = $"Orcamento da OS #{orcamento.IdOS}",
            Corpo = $"O orcamento da OS #{orcamento.IdOS} esta disponivel. Valor total: {orcamento.ValorTotal:C}.",
            Status = EStatusEmailOutbox.Pendente,
            Tentativas = 0,
            CriadoEm = DateTime.UtcNow
        };

        await _emailOutboxRepositorio.CriarAsync(email);
    }

    private async Task<Resultado> AtualizarStatusParaAguardandoAprovacaoAsync(Dominio.Model.OrdemServico ordemServico)
    {
        if (ordemServico.Status == EStatusOrdemServico.AguardandoAprovacao)
            return Resultado.Sucesso();

        var agora = DateTime.UtcNow;
        var historico = new List<Status>();

        if (ordemServico.Status == EStatusOrdemServico.Recebida)
        {
            historico.Add(CriarStatus(ordemServico, EStatusOrdemServico.EmDiagnostico, agora));
            historico.Add(CriarStatus(ordemServico, EStatusOrdemServico.AguardandoAprovacao, agora.AddTicks(1)));
            ordemServico.AtualizadoEm = agora.AddTicks(1);
        }
        else
        {
            historico.Add(CriarStatus(ordemServico, EStatusOrdemServico.AguardandoAprovacao, agora));
            ordemServico.AtualizadoEm = agora;
        }

        ordemServico.Status = EStatusOrdemServico.AguardandoAprovacao;
        if (!await _ordemServicoRepositorio.AtualizarAsync(ordemServico))
            return Resultado.Falha("Nao foi possivel atualizar o status da ordem de servico.");

        foreach (var status in historico)
            await _statusRepositorio.CriarAsync(status);

        return Resultado.Sucesso();
    }

    private static Status CriarStatus(Dominio.Model.OrdemServico ordemServico, EStatusOrdemServico status, DateTime atualizadoEm)
    {
        return new Status
        {
            IdOS = ordemServico.Id,
            IdFuncionario = ordemServico.IdFuncionario,
            StatusAtual = status,
            UpdatedAt = atualizadoEm
        };
    }

    private static ObterOrcamentoDto MapearParaDto(Dominio.Model.Orcamento orcamento)
    {
        return new ObterOrcamentoDto
        {
            Id = orcamento.Id,
            IdOS = orcamento.IdOS,
            ValorMaoDeObra = orcamento.ValorMaoDeObra,
            ValorPecas = orcamento.ValorPecas,
            ValorTotal = orcamento.ValorTotal,
            Status = (int)orcamento.Status,
            CriadoEm = orcamento.CriadoEm,
            AtualizadoEm = orcamento.AtualizadoEm
        };
    }
}
