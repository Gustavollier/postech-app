using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IVeiculosRepositorio
    {
        Task<IEnumerable<Veiculo>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task<Veiculo?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken = default);
        Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(int clienteId, CancellationToken cancellationToken = default);
        Task<int> CriarAsync(Veiculo veiculo, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAsync(Veiculo veiculo, CancellationToken cancellationToken = default);
        Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default);
    }
}
