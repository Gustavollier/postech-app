using PosTechChallenge.Aplicacao.Dto.ItemOS;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.UseCases.ItemOS;

public sealed class ObterItemOSUseCase
{
    private readonly IItemsRepositorio _itemsRepositorio;
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;

    public ObterItemOSUseCase(
        IItemsRepositorio itemsRepositorio,
        IOrdemServicoRepositorio ordemServicoRepositorio)
    {
        _itemsRepositorio = itemsRepositorio;
        _ordemServicoRepositorio = ordemServicoRepositorio;
    }

    public async Task<Resultado<IEnumerable<ObterItemOSDto>>> ObterPorOrdemServicoIdAsync(int ordemServicoId)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(ordemServicoId);

            if (ordemServico == null)
                return Resultado<IEnumerable<ObterItemOSDto>>.Falha($"OS com ID {ordemServicoId} não encontrada.");

            var itens = await _itemsRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId);
            var lista = itens.Select(Mapear).ToList();

            if (!lista.Any())
                return Resultado<IEnumerable<ObterItemOSDto>>.Falha($"Nenhum item encontrado para a OS {ordemServicoId}.");

            return Resultado<IEnumerable<ObterItemOSDto>>.Sucesso(lista);
        }
        catch (Exception ex)
        {
            return Resultado<IEnumerable<ObterItemOSDto>>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterItemOSDto>> ObterPorIdAsync(int ordemServicoId, int id)
    {
        try
        {
            var item = await _itemsRepositorio.ObterPorIdAsync(id);

            if (item == null)
                return Resultado<ObterItemOSDto>.Falha($"Item da OS com ID {id} não encontrado.");

            if (item.IdOS != ordemServicoId)
                return Resultado<ObterItemOSDto>.Falha($"Item {id} não pertence à OS {ordemServicoId}.");

            return Resultado<ObterItemOSDto>.Sucesso(Mapear(item));
        }
        catch (Exception ex)
        {
            return Resultado<ObterItemOSDto>.Falha(ex.Message);
        }
    }

    private static ObterItemOSDto Mapear(Dominio.Model.ItemOS item)
    {
        return new ObterItemOSDto
        {
            Id = item.Id,
            IdOS = item.IdOS,
            TipoItem = item.TipoItem,
            QuantidadeItem = item.QuantidadeItem,
            IdFuncionario = item.IdFuncionario > 0 ? item.IdFuncionario : null,
            IdPeca = item.IdPeca > 0 ? item.IdPeca : null
        };
    }
}
