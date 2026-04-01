using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Interfaces.Repositorios;

namespace PosTechChallenge.Applicacao.UseCases.Funcionario;

public class ObterFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public ObterFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task<ObterFuncionarioDto?> ObterPorCpfAsync(string cpf)
    {
        var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
        var funcionario = funcionarios.FirstOrDefault(f => f.CPF == cpf);

        if (funcionario == null)
            return null;

        return MapearParaDto(funcionario);
    }

    public async Task<ObterFuncionarioDto?> ObterPorNomeAsync(string nome)
    {
        var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
        var funcionario = funcionarios.FirstOrDefault(f => f.Nome == nome);

        if (funcionario == null)
            return null;

        return MapearParaDto(funcionario);
    }

    public async Task<IEnumerable<ObterFuncionarioDto>> ObterTodosAsync()
    {
        var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
        return funcionarios.Select(MapearParaDto);
    }

    private static ObterFuncionarioDto MapearParaDto(Dominio.Model.Funcionario funcionario)
    {
        return new ObterFuncionarioDto
        {
            Id = funcionario.Id,
            Nome = funcionario.Nome,
            Contato = funcionario.Contato,
            CPF = funcionario.CPF,
            Cargo = funcionario.Cargo,
            ValorHora = funcionario.ValorHora
        };
    }
}
