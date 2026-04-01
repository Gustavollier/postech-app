using PosTechChallenge.Applicacao.Dto.Funcionario;

namespace PosTechChallenge.Applicacao.Interface.Services;

public interface IFuncionarioService
{
    Task CriarAsync(CriarFuncionarioDto funcionarioDto);
    Task<ObterFuncionarioDto?> ObterPorCpfAsync(string cpf);
    Task<ObterFuncionarioDto?> ObterPorNomeAsync(string nome);
    Task<IEnumerable<ObterFuncionarioDto>> ObterTodosAsync();
    Task<bool> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto);
    Task<bool> DeletarAsync(string cpf);
}
