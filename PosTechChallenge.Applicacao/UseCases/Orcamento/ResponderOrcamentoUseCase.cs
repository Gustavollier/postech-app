using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.UseCases.Orcamento;

public sealed class ResponderOrcamentoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly IOrcamentoRepositorio _orcamentoRepositorio;
    private readonly IStatusRepositorio _statusRepositorio;

    public ResponderOrcamentoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        IOrcamentoRepositorio orcamentoRepositorio,
        IStatusRepositorio statusRepositorio)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _orcamentoRepositorio = orcamentoRepositorio;
        _statusRepositorio = statusRepositorio;
    }

    public async Task<Resultado> ResponderAsync(int ordemServicoId, ResponderOrcamentoDto dto)
    {
        try
        {
            if (dto.Status is not (EStatusOrcamento.Aprovado or EStatusOrcamento.Rejeitado))
                return Resultado.Falha("Resposta do orcamento deve ser Aprovado ou Rejeitado.");

            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                return Resultado.Falha($"Ordem de servico com ID {ordemServicoId} nao encontrada.");

            if (ordemServico.Status != EStatusOrdemServico.AguardandoAprovacao)
                return Resultado.Falha("Orcamento so pode ser respondido quando a OS esta aguardando aprovacao.");

            var orcamento = await _orcamentoRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId);
            if (orcamento == null)
                return Resultado.Falha($"Orcamento da OS {ordemServicoId} nao encontrado.");

            if (orcamento.Status != EStatusOrcamento.Pendente)
                return Resultado.Falha("Orcamento ja foi respondido.");

            var novoStatusOs = dto.Status == EStatusOrcamento.Aprovado
                ? EStatusOrdemServico.EmExecucao
                : EStatusOrdemServico.Cancelada;

            var agora = DateTime.UtcNow;
            orcamento.Status = dto.Status;
            orcamento.AtualizadoEm = agora;

            if (!await _orcamentoRepositorio.AtualizarAsync(orcamento))
                return Resultado.Falha("Nao foi possivel atualizar o orcamento.");

            ordemServico.Status = novoStatusOs;
            ordemServico.AtualizadoEm = agora;

            if (!await _ordemServicoRepositorio.AtualizarAsync(ordemServico))
                return Resultado.Falha("Nao foi possivel atualizar o status da ordem de servico.");

            await _statusRepositorio.CriarAsync(new Status
            {
                IdOS = ordemServico.Id,
                IdFuncionario = ordemServico.IdFuncionario,
                StatusAtual = novoStatusOs,
                UpdatedAt = agora
            });

            return Resultado.Sucesso("Resposta do orcamento registrada com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }
}
