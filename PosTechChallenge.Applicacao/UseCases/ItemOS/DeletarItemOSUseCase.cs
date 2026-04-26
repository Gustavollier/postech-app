using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.Services;

namespace PosTechChallenge.Aplicacao.UseCases.ItemOS;

public sealed class DeletarItemOSUseCase
{
    private readonly IItemsRepositorio _itemsRepositorio;
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly ItemOSDomainService _itemOSDomainService;

    public DeletarItemOSUseCase(
        IItemsRepositorio itemsRepositorio,
        IOrdemServicoRepositorio ordemServicoRepositorio,
        ItemOSDomainService itemOSDomainService)
    {
        _itemsRepositorio = itemsRepositorio;
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _itemOSDomainService = itemOSDomainService;
    }

    public async Task<Resultado> DeletarAsync(int ordemServicoId, int id)
    {
        try
        {
            var item = await _itemsRepositorio.ObterPorIdAsync(id);

            if (item == null)
                return Resultado.Falha($"Item da OS com ID {id} não encontrado.");

            if (item.IdOS != ordemServicoId)
                return Resultado.Falha($"Item {id} não pertence à OS {ordemServicoId}.");

            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(ordemServicoId);

            if (ordemServico == null)
                return Resultado.Falha($"OS com ID {ordemServicoId} não encontrada.");

            var validacao = _itemOSDomainService.ValidarRemocao(ordemServico);
            if (!validacao.IsValid)
                return validacao;

            var removido = await _itemsRepositorio.DeletarAsync(id);
            if (!removido)
                return Resultado.Falha("Não foi possível remover o item da OS.");

            await AtualizarDataOSAsync(ordemServico);

            return Resultado.Sucesso("Item da OS removido com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }

    private async Task AtualizarDataOSAsync(Dominio.Model.OrdemServico ordemServico)
    {
        ordemServico.AtualizadoEm = DateTime.UtcNow;
        await _ordemServicoRepositorio.AtualizarAsync(ordemServico);
    }
}
