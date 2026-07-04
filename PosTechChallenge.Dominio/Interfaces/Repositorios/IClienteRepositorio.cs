using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios;

public interface IClienteRepositorio
{
    Task<IEnumerable<Cliente>> ObterTodosAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> ObterQuantidadeClientesAsync(CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default);
    Task<int> CriarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<bool> DesativarAsync(int id, CancellationToken cancellationToken = default);
}
