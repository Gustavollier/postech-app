using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IOrdemServicoRepositorio
    {
        Task<IEnumerable<OrdemServico>> ObterTodosAsync();
        Task<OrdemServico?> ObterPorIdAsync(int id);
        Task<int> CriarAsync(OrdemServico ordemServico);
        Task<bool> AtualizarAsync(OrdemServico ordemServico);
        Task<bool> DeletarAsync(int id);
    }
}
