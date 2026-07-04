using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Aplicacao.UseCases.Funcionario;

public class CriarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly ISegurancaRepositorio _segurancaRepositorio;
    private readonly IUnitOfWork _unitOfWork;

    public CriarFuncionarioUseCase(
        IFuncionarioRepositorio funcionarioRepositorio,
        ISegurancaRepositorio segurancaRepositorio,
        IUnitOfWork unitOfWork)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _segurancaRepositorio = segurancaRepositorio;
        _unitOfWork = unitOfWork;
    }

    public async Task<Resultado> CriarAsync(CriarFuncionarioDto funcionarioDto, CancellationToken cancellationToken = default)
    {
        try
        {
            SenhaValueObject senhaValueObject = new(funcionarioDto.Senha);

            if (senhaValueObject.ConfirmarSenha(funcionarioDto.ConfirmacaoSenha) is false)
                return Resultado.Falha("As senhas não conferem.");

            Dominio.Model.Funcionario funcionario = new()
            {
                Nome = funcionarioDto.Nome,
                Contato = funcionarioDto.Contato,
                CPF = funcionarioDto.CPF,
                Cargo = funcionarioDto.Cargo,
                ValorHora = funcionarioDto.ValorHora
            };

            // Funcionário e senha gravados na mesma transação.
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var funcionarioId = await _funcionarioRepositorio.CriarAsync(funcionario, cancellationToken);
            var senhaHash = PasswordHasher.HashPassword(senhaValueObject.Valor);

            await _segurancaRepositorio.SalvarSenhaAsync(funcionarioId, senhaHash, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Resultado.Sucesso("Funcionário criado com sucesso.");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            return Resultado.Falha("Erro ao criar funcionário.");
        }
    }
}
