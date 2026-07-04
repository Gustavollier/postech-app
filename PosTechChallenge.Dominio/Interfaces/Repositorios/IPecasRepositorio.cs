using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IPecasRepositorio
    {
        Task<IEnumerable<Pecas>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task<Pecas?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<int> CriarAsync(Pecas peca, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAsync(Pecas peca, CancellationToken cancellationToken = default);
        Task<bool> AjustarEstoqueAsync(int id, int quantidadeEstoque, DateTime atualizadoEm, CancellationToken cancellationToken = default);
        Task<bool> DeletarAsync(int id, DateTime atualizadoEm, CancellationToken cancellationToken = default);
    }
}
