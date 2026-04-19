using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IFuncionarioService
{
    Task<Resultado> CriarAsync(CriarFuncionarioDto funcionarioDto);
    Task<Resultado<ObterFuncionarioDto>> ObterPorCpfAsync(string cpf);
    Task<Resultado<ObterFuncionarioDto>> ObterPorNomeAsync(string nome);
    Task<Resultado<IEnumerable<ObterFuncionarioDto>>> ObterTodosAsync();
    Task<Resultado> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto);
    Task<Resultado> DeletarAsync(string cpf);
}
