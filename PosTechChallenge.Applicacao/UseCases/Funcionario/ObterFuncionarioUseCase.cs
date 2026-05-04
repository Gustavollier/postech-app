using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.UseCases.Funcionario;

public class ObterFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;

    public ObterFuncionarioUseCase(IFuncionarioRepositorio funcionarioRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
    }

    public async Task<Resultado<ObterFuncionarioDto>> ObterPorCpfAsync(string cpf)
    {
        try
        {
            var funcionario = await _funcionarioRepositorio.ObterPorCPFAsync(cpf);

            if (funcionario == null)
                return Resultado<ObterFuncionarioDto>.Falha($"Funcionário com CPF {cpf} não encontrado.");

            return Resultado<ObterFuncionarioDto>.Sucesso(MapearParaDto(funcionario));
        }
        catch (Exception ex)
        {
            return Resultado<ObterFuncionarioDto>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterFuncionarioDto>> ObterPorNomeAsync(string nome)
    {
        try
        {                                                       
            var funcionario = await _funcionarioRepositorio.ObterPorNomeAsync(nome);

            if (funcionario == null)
                return Resultado<ObterFuncionarioDto>.Falha($"Funcionário com nome '{nome}' não encontrado.");

            return Resultado<ObterFuncionarioDto>.Sucesso(MapearParaDto(funcionario));
        }
        catch (Exception ex)
        {
            return Resultado<ObterFuncionarioDto>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<IEnumerable<ObterFuncionarioDto>>> ObterTodosAsync()
    {
        try
        {
            var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();

            if (funcionarios == null || !funcionarios.Any())
                return Resultado<IEnumerable<ObterFuncionarioDto>>.Falha("Nenhum funcionário encontrado.");

            var dtos = funcionarios.Select(MapearParaDto).ToList();
            return Resultado<IEnumerable<ObterFuncionarioDto>>.Sucesso(dtos);
        }
        catch (Exception ex)
        {
            return Resultado<IEnumerable<ObterFuncionarioDto>>.Falha(ex.Message);
        }
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
