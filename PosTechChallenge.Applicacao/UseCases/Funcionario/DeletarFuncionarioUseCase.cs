using PosTechChallenge.Dominio.Interfaces.Repositorios;

namespace PosTechChallenge.Applicacao.UseCases.Funcionario;

public class DeletarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public DeletarFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task<bool> DeletarPorCpfAsync(string cpf)
    {
        var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
        var funcionario = funcionarios.FirstOrDefault(f => f.CPF == cpf);

        if (funcionario == null)
            return false;

        return await _funcionarioRepositorio.DeletarAsync(funcionario.Id);
    }
}
