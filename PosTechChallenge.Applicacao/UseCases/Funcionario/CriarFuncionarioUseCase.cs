using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Applicacao.UseCases.Funcionario;
public class CriarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public CriarFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task<Resultado> CriarAsync(CriarFuncionarioDto funcionarioDto)
    {
        try
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
            return Resultado.Sucesso("Funcionário criado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }
}
