using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IStatusRepositorio
    {
        Task<IEnumerable<Status>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Status>> ObterPorOrdemServicoIdAsync(int ordemServicoId, CancellationToken cancellationToken = default);
        Task<Status?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<int> CriarAsync(Status status, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAsync(Status status, CancellationToken cancellationToken = default);
    }
}
