using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Interfaces.Repositorios;

namespace PosTechChallenge.Applicacao.UseCases.Funcionario;

public class AtualizarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public AtualizarFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task<bool> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto)
    {
        var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
        var funcionarioExistente = funcionarios.FirstOrDefault(f => f.CPF == cpf);

        if (funcionarioExistente == null)
            return false;

        funcionarioExistente.Nome = funcionarioDto.Nome;
        funcionarioExistente.Contato = funcionarioDto.Contato;
        funcionarioExistente.Cargo = funcionarioDto.Cargo;
        funcionarioExistente.ValorHora = funcionarioDto.ValorHora;

        return await _funcionarioRepositorio.AtualizarAsync(funcionarioExistente);
    }
}
