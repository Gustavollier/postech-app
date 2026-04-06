using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Applicacao.UseCases.Funcionario;

public class DeletarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public DeletarFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task<Resultado> DeletarPorCpfAsync(string cpf)
    {
        try
        {
            var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
            var funcionario = funcionarios.FirstOrDefault(f => f.CPF == cpf);

            if (funcionario == null)
                return Resultado.Falha($"Funcionário com CPF {cpf} não encontrado.");

            await _funcionarioRepositorio.DeletarAsync(funcionario.Id);
            return Resultado.Sucesso("Funcionário deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }
}
