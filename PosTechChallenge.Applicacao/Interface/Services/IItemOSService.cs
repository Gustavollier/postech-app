using PosTechChallenge.Aplicacao.Dto.ItemOS;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IItemOSService
{
    Task<Resultado> CriarAsync(CriarItemOSDto dto);
    Task<Resultado<IEnumerable<ObterItemOSDto>>> ObterPorOrdemServicoIdAsync(int ordemServicoId);
    Task<Resultado<ObterItemOSDto>> ObterPorIdAsync(int ordemServicoId, int id);
    Task<Resultado> AtualizarAsync(int ordemServicoId, int id, AtualizarItemOSDto dto);
    Task<Resultado> DeletarAsync(int ordemServicoId, int id);
}
