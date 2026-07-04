using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IClienteService
{
    Task<Resultado> CriarAsync(CriarClienteDto clienteDto, CancellationToken cancellationToken = default);
    Task<Resultado<ClienteDto>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Resultado<ClienteDto>> ObterPorCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default);
    Task<Resultado<ObterClienteDto>> ObterTodosAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Resultado> AtualizarAsync(int id, AtualizarClienteDto clienteDto, CancellationToken cancellationToken = default);
    Task<Resultado> DesativarAsync(int id, CancellationToken cancellationToken = default);
}
