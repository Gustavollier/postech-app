using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IOrdemServicoService
{
    Task<Resultado> CriarAsync(CriarOrdemServicoDto dto);
    Task<Resultado<IEnumerable<ObterOrdemServicoDto>>> ObterTodosAsync(EStatusOrdemServico? status, int pageSize = 10, int page = 1);
    Task<Resultado<decimal>> ObterValorPorIdAsync(int id);
    Task<Resultado<ObterOrdemServicoDto>> ObterPorIdAsync(int id);
    Task<Resultado<ObterStatusOrdemServicoDto>> ObterStatusAsync(int id);
    Task<Resultado> AtualizarAsync(int id, AtualizarOrdemServicoDto dto);
    Task<Resultado> AtualizarStatusAsync(int id, AtualizarStatusOrdemServicoDto dto);
    Task<Resultado> DeletarAsync(int id);
}
