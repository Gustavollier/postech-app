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

    public AtualizarStatusOrdemServicoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        IStatusRepositorio statusRepositorio,
        IItemsRepositorio itemsRepositorio,
        IPecasRepositorio pecasRepositorio,
        OrdemServicoDomainService ordemServicoDomainService)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _statusRepositorio = statusRepositorio;
        _itemsRepositorio = itemsRepositorio;
        _pecasRepositorio = pecasRepositorio;
        _ordemServicoDomainService = ordemServicoDomainService;
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

            if (novoStatus is EStatusOrdemServico.Finalizada || EStatusOrdemServico.Cancelada)
            {
                Resultado baixouEstoque = await BaixarEstoqueAsync(id);
                
                if (baixouEstoque.IsValid is false)
                    return baixouEstoque;
            }

            ordemServico.Status = novoStatus;
            ordemServico.AtualizadoEm = DateTime.UtcNow;

            bool atualizado = await _ordemServicoRepositorio.AtualizarAsync(ordemServico);

            if (atualizado is false)
                return Resultado.Falha("Não foi possível atualizar o status da ordem de serviço.");

            await _statusRepositorio.CriarAsync(new Status
            {
                IdOS = ordemServico.Id,
                IdFuncionario = dto.IdFuncionario,
                StatusAtual = ordemServico.Status,
                UpdatedAt = ordemServico.AtualizadoEm
            });

            return Resultado.Sucesso("Status da ordem de serviço atualizado com sucesso.");
        }
        catch (Exception ex)
        {
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
