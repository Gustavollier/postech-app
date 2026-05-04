using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IOrdemServicoRepositorio
    {
        Task<IEnumerable<OrdemServico>> ObterTodosAsync(int pageSize, int page);
        Task<IEnumerable<OrdemServico>> ObterPorClienteIdAsync(int idCliente, int pageSize, int page);
        Task<decimal> ObterValorPorIdAsync(int id);
        Task<OrdemServico?> ObterPorIdAsync(int id);
        Task<int> CriarAsync(OrdemServico ordemServico);
        Task<bool> AtualizarAsync(OrdemServico ordemServico);
        Task<bool> DeletarAsync(int id);
    }
}
