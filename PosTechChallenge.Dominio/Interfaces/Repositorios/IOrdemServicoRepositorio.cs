using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IOrdemServicoRepositorio
    {
        Task<IEnumerable<OrdemServico>> ObterTodosAsync(int pageSize, int page, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrdemServico>> ObterOrdenadoPorStatusAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<OrdemServico>> ObterPorClienteIdAsync(int idCliente, int pageSize, int page, CancellationToken cancellationToken = default);
        Task<decimal> ObterValorPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<OrdemServico?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<int> CriarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default);
        Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default);
    }
}
