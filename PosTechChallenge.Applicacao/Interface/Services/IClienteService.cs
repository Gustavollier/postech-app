using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IClienteService
{
    Task<Resultado> CriarAsync(CriarClienteDto clienteDto);
    Task<Resultado<ClienteDto>> ObterPorIdAsync(int id);
    Task<Resultado<ClienteDto>> ObterPorCpfCnpjAsync(string cpfCnpj);
    Task<Resultado<ObterClienteDto>> ObterTodosAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Resultado> AtualizarAsync(int id, AtualizarClienteDto clienteDto);
    Task<Resultado> DesativarAsync(int id);
}