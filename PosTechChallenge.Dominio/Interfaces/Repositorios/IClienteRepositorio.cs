using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios;

public interface IClienteRepositorio
{
    Task<IEnumerable<Cliente>> ObterTodosAsync();
    Task<Cliente?> ObterPorIdAsync(int id);
    Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj);
    Task<int> CriarAsync(Cliente cliente);
    Task<bool> AtualizarAsync(Cliente cliente);
    Task<bool> DesativarAsync(int id);
}