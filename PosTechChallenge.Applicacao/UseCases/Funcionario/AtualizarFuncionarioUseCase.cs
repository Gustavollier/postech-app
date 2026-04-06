using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Applicacao.UseCases.Funcionario;

public class AtualizarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public AtualizarFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task<Resultado> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto)
    {
        try
        {
            var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
            var funcionarioExistente = funcionarios.FirstOrDefault(f => f.CPF == cpf);

            if (funcionarioExistente == null)
                return Resultado.Falha($"Funcionário com CPF {cpf} não encontrado.");

            funcionarioExistente.Nome = funcionarioDto.Nome;
            funcionarioExistente.Contato = funcionarioDto.Contato;
            funcionarioExistente.Cargo = funcionarioDto.Cargo;
            funcionarioExistente.ValorHora = funcionarioDto.ValorHora;

            await _funcionarioRepositorio.AtualizarAsync(funcionarioExistente);
            return Resultado.Sucesso("Funcionário atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }
}
