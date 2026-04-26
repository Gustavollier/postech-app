using PosTechChallenge.Aplicacao.Dto.ItemOS;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.Services;

namespace PosTechChallenge.Aplicacao.UseCases.ItemOS;

public sealed class AtualizarItemOSUseCase
{
    private readonly IItemsRepositorio _itemsRepositorio;
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly ItemOSDomainService _itemOSDomainService;

    public AtualizarItemOSUseCase(
        IItemsRepositorio itemsRepositorio,
        IOrdemServicoRepositorio ordemServicoRepositorio,
        ItemOSDomainService itemOSDomainService)
    {
        _itemsRepositorio = itemsRepositorio;
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _itemOSDomainService = itemOSDomainService;
    }

    public async Task<Resultado> AtualizarAsync(int ordemServicoId, int id, AtualizarItemOSDto dto)
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

            item.TipoItem = dto.TipoItem;
            item.QuantidadeItem = dto.QuantidadeItem;
            item.IdFuncionario = dto.IdFuncionario ?? 0;
            item.IdPeca = dto.IdPeca ?? 0;

            var validacao = await _itemOSDomainService.ValidarAtualizacaoAsync(ordemServico, item);

            if (!validacao.IsValid)
                return validacao;

            var atualizado = await _itemsRepositorio.AtualizarAsync(item);
            
            if (!atualizado)
                return Resultado.Falha("Não foi possível atualizar o item da OS.");

            await AtualizarDataOSAsync(ordemServico);

            return Resultado.Sucesso("Item da OS atualizado com sucesso.");
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
