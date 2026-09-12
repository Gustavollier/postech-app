using Microsoft.Extensions.Logging;
using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.Services;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.UseCases.OrdemServico;

public sealed class AtualizarStatusOrdemServicoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly IStatusRepositorio _statusRepositorio;
    private readonly IItemsRepositorio _itemsRepositorio;
    private readonly IPecasRepositorio _pecasRepositorio;
    private readonly OrdemServicoDomainService _ordemServicoDomainService;
    private readonly ILogger<AtualizarStatusOrdemServicoUseCase> _logger;

    public AtualizarStatusOrdemServicoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        IStatusRepositorio statusRepositorio,
        IItemsRepositorio itemsRepositorio,
        IPecasRepositorio pecasRepositorio,
        OrdemServicoDomainService ordemServicoDomainService,
        ILogger<AtualizarStatusOrdemServicoUseCase> logger)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _statusRepositorio = statusRepositorio;
        _itemsRepositorio = itemsRepositorio;
        _pecasRepositorio = pecasRepositorio;
        _ordemServicoDomainService = ordemServicoDomainService;
        _logger = logger;
    }

    public async Task<Resultado> AtualizarAsync(int id, AtualizarStatusOrdemServicoDto dto)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(id);

            if (ordemServico == null)
                return Resultado.Falha($"Ordem de serviço com ID {id} não encontrada.");

            var novoStatus = dto.Status;

            var validacao = await _ordemServicoDomainService.ValidarTransicaoStatusAsync(ordemServico, novoStatus, dto.IdFuncionario);
            
            if (validacao.IsValid is false)
                return validacao;

            if (novoStatus is EStatusOrdemServico.Finalizada)
            {
                Resultado baixouEstoque = await BaixarEstoqueAsync(id);
                
                if (baixouEstoque.IsValid is false)
                    return baixouEstoque;
            }

            var statusAnterior = ordemServico.Status;
            var agora = DateTime.UtcNow;

            // Quanto tempo a OS permaneceu no status anterior. É a métrica que
            // alimenta o painel "tempo médio de execução por status".
            var duracaoNoStatusAnterior = agora - ordemServico.AtualizadoEm;

            ordemServico.Status = novoStatus;
            ordemServico.AtualizadoEm = agora;

            bool atualizado = await _ordemServicoRepositorio.AtualizarAsync(ordemServico);

            if (atualizado is false)
            {
                // Nomes em snake_case propositalmente: o JsonConsole emite as
                // propriedades do template sob "State", e as queries do Datadog
                // referenciam @State.evento, @State.from_status etc.
                _logger.LogError(
                    "Falha ao processar ordem de serviço. {evento} {os_id} {from_status} {to_status} {motivo}",
                    "FalhaProcessamentoOrdemServico",
                    ordemServico.Id,
                    statusAnterior.ToString(),
                    novoStatus.ToString(),
                    "repositorio_nao_atualizou");

                return Resultado.Falha("Não foi possível atualizar o status da ordem de serviço.");
            }

            await _statusRepositorio.CriarAsync(new Status
            {
                IdOS = ordemServico.Id,
                IdFuncionario = dto.IdFuncionario,
                StatusAtual = ordemServico.Status,
                UpdatedAt = ordemServico.AtualizadoEm
            });

            _logger.LogInformation(
                "Transição de status da ordem de serviço. {evento} {os_id} {from_status} {to_status} {duracao_ms}",
                "TransicaoStatusOrdemServico",
                ordemServico.Id,
                statusAnterior.ToString(),
                novoStatus.ToString(),
                duracaoNoStatusAnterior.TotalMilliseconds);

            return Resultado.Sucesso("Status da ordem de serviço atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Falha ao processar ordem de serviço. {evento} {os_id} {to_status}",
                "FalhaProcessamentoOrdemServico",
                id,
                dto.Status.ToString());

            return Resultado.Falha(ex.Message);
        }
    }

    private async Task<Resultado> BaixarEstoqueAsync(int ordemServicoId)
    {
        var itens = await _itemsRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId);

        foreach (var item in itens.Where(i => i.IdPeca > 0))
        {
            Pecas? peca = await _pecasRepositorio.ObterPorIdAsync(item.IdPeca);

            if (peca is null)
                return Resultado.Falha($"Peça com ID {item.IdPeca} não encontrada.");

            int novaQuantidade = peca.QuantidadeEstoque - item.QuantidadeItem;

            if (novaQuantidade < 0)
                return Resultado.Falha($"Estoque insuficiente para a peça {peca.Id}.");

            bool ajustado = await _pecasRepositorio.AjustarEstoqueAsync(peca.Id, novaQuantidade, DateTime.UtcNow);

            if (ajustado is false)
                return Resultado.Falha($"Não foi possível baixar o estoque da peça {peca.Id}.");
        }

        return Resultado.Sucesso();
    }
}
