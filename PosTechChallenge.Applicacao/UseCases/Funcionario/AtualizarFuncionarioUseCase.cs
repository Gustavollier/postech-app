using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Dominio.Interfaces;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.UseCases.Funcionario;

public class AtualizarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarFuncionarioUseCase(
        IFuncionarioRepositorio funcionarioRepositorio,
        IUnitOfWork unitOfWork)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _unitOfWork = unitOfWork;
    }

    public async Task<Resultado> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var funcionarioExistente = await _funcionarioRepositorio.ObterPorCPFAsync(cpf, cancellationToken);

            if (funcionarioExistente == null)
                return Resultado.Falha($"Funcionário com CPF {cpf} não encontrado.");

            funcionarioExistente.Nome = funcionarioDto.Nome;
            funcionarioExistente.Contato = funcionarioDto.Contato;
            funcionarioExistente.Cargo = funcionarioDto.Cargo;
            funcionarioExistente.ValorHora = funcionarioDto.ValorHora;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _funcionarioRepositorio.AtualizarAsync(funcionarioExistente, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Resultado.Sucesso("Funcionário atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            return Resultado.Falha(ex.Message);
        }
    }
}
