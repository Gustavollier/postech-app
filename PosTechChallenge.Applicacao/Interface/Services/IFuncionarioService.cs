using PosTechChallenge.Applicacao.Dto.Funcionario;

namespace PosTechChallenge.Applicacao.Interface.Services;
public interface IFuncionarioService
{
    Task CriarAsync(CriarFuncionarioDto funcionarioDto);
}
