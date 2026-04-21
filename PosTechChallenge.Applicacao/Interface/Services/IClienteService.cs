using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IClienteService
{
    Task<Resultado> CriarAsync(CriarClienteDto clienteDto);
    Task<Resultado<ObterClienteDto>> ObterPorIdAsync(int id);
    Task<Resultado<ObterClienteDto>> ObterPorCpfCnpjAsync(string cpfCnpj);
    Task<Resultado<IEnumerable<ObterClienteDto>>> ObterTodosAsync();
    Task<Resultado> AtualizarAsync(int id, AtualizarClienteDto clienteDto);
    Task<Resultado> DesativarAsync(int id);
}