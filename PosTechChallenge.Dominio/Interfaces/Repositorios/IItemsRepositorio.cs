using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IItemsRepositorio
    {
        Task<IEnumerable<ItemOS>> ObterTodosAsync();
        Task<IEnumerable<ItemOS>> ObterPorOrdemServicoIdAsync(int ordemServicoId);
        Task<ItemOS?> ObterPorIdAsync(int id);
        Task<int> CriarAsync(ItemOS item);
        Task<bool> AtualizarAsync(ItemOS item);
        Task<bool> DeletarAsync(int id);
    }
}
