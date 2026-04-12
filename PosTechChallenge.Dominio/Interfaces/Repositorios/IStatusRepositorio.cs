using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IStatusRepositorio
    {
        Task<IEnumerable<Status>> ObterTodosAsync();
        Task<Status?> ObterPorIdAsync(int id);
        Task<int> CriarAsync(Status status);
        Task<bool> AtualizarAsync(Status status);
    }
}
