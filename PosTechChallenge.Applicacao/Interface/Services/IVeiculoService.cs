using PosTechChallenge.Aplicacao.Dto.Veiculo;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IVeiculoService
{
    Task<Resultado> CriarAsync(CriarVeiculoDto veiculoDto);
    Task<Resultado<ObterVeiculoDto>> ObterPorIdAsync(int id);
    Task<Resultado<ObterVeiculoDto>> ObterPorPlacaAsync(string placa);
    Task<Resultado<IEnumerable<ObterVeiculoDto>>> ObterPorClienteIdAsync(int clienteId);
    Task<Resultado> AtualizarAsync(int id, AtualizarVeiculoDto veiculoDto);
}