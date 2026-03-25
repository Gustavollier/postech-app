using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Interfaces.Repositorios;

namespace PosTechChallenge.Applicacao.UseCases.Funcionario;
public class CriarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public CriarFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task CriarAsync(CriarFuncionarioDto funcionarioDto)
    {

        Dominio.Model.Funcionario funcionario = new()
        {
            Nome = funcionarioDto.Nome,
            Contato = funcionarioDto.Contato,
            CPF = funcionarioDto.CPF,
            Cargo = funcionarioDto.Cargo,
            ValorHora = funcionarioDto.ValorHora
        };

        await _funcionarioRepositorio.CriarAsync(funcionario);
    }
}
