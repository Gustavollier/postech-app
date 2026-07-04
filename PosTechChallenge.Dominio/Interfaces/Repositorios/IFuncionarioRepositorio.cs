using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IFuncionarioRepositorio
    {
        Task<IEnumerable<Funcionario>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task<Funcionario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Funcionario?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
        Task<Funcionario?> ObterPorCPFAsync(string CPF, CancellationToken cancellationToken = default);
        Task<int> CriarAsync(Funcionario funcionario, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAsync(Funcionario funcionario, CancellationToken cancellationToken = default);
        Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default);
    }
}
