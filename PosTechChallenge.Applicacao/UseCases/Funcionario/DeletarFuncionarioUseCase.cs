using PosTechChallenge.Dominio.Interfaces;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.UseCases.Funcionario;

public class DeletarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly IUnitOfWork _unitOfWork;

    public DeletarFuncionarioUseCase(
        IFuncionarioRepositorio funcionarioRepositorio,
        IUnitOfWork unitOfWork)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _unitOfWork = unitOfWork;
    }

    public async Task<Resultado> DeletarPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        try
        {
            var funcionario = await _funcionarioRepositorio.ObterPorCPFAsync(cpf, cancellationToken);

            if (funcionario == null)
                return Resultado.Falha($"Funcionário com CPF {cpf} não encontrado.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _funcionarioRepositorio.DeletarAsync(funcionario.Id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Resultado.Sucesso("Funcionário deletado com sucesso.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            return Resultado.Falha(ex.Message);
        }
    }
}
