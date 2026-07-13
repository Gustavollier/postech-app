using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IFuncionarioService
{
    Task<Resultado> CriarAsync(CriarFuncionarioDto funcionarioDto, CancellationToken cancellationToken = default);
    Task<Resultado<ObterFuncionarioDto>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Resultado<ObterFuncionarioDto>> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<Resultado<ObterFuncionarioDto>> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<Resultado<IEnumerable<ObterFuncionarioDto>>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<Resultado> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto, CancellationToken cancellationToken = default);
    Task<Resultado> DeletarAsync(string cpf, CancellationToken cancellationToken = default);
}
