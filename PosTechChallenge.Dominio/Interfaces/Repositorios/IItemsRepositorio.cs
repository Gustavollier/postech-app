using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IItemsRepositorio
    {
        Task<IEnumerable<ItemOS>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ItemOS>> ObterPorOrdemServicoIdAsync(int ordemServicoId, CancellationToken cancellationToken = default);
        Task<ItemOS?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<int> CriarAsync(ItemOS item, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAsync(ItemOS item, CancellationToken cancellationToken = default);
        Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default);
    }
}
