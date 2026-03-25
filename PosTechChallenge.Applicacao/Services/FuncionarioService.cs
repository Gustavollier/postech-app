using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Applicacao.UseCases.Funcionario;

namespace PosTechChallenge.Applicacao.Services;
public sealed class FuncionarioService : IFuncionarioService
{
    private readonly CriarFuncionarioUseCase _criarFuncionarioUseCase;

    public FuncionarioService(CriarFuncionarioUseCase criarFuncionarioUseCase)
    {
        _criarFuncionarioUseCase = criarFuncionarioUseCase;
    }

    public async Task CriarAsync(CriarFuncionarioDto funcionarioDto) 
        => await _criarFuncionarioUseCase.CriarAsync(funcionarioDto);
}
