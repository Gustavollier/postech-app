using Moq;
using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Aplicacao.UseCases.Funcionario;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.Application;

public class CriarFuncionarioUseCaseTests
{
    private readonly Mock<IFuncionarioRepositorio> _funcionarioRepositorio = new();
    private readonly Mock<ISegurancaRepositorio> _segurancaRepositorio = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CriarFuncionarioUseCase _useCase;

    public CriarFuncionarioUseCaseTests()
    {
        _useCase = new CriarFuncionarioUseCase(
            _funcionarioRepositorio.Object,
            _segurancaRepositorio.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarFuncionarioESenhaEComitar()
    {
        string? senhaHash = null;

        _funcionarioRepositorio
            .Setup(r => r.CriarAsync(It.IsAny<Funcionario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _segurancaRepositorio
            .Setup(r => r.SalvarSenhaAsync(10, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<int, string, CancellationToken>((_, hash, _) => senhaHash = hash)
            .Returns(Task.CompletedTask);

        var resultado = await _useCase.CriarAsync(CriarDto());

        Assert.True(resultado.IsValid);
        Assert.NotNull(senhaHash);
        Assert.True(PasswordHasher.VerifyPassword("Senha@123", senhaHash!));
        _unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_SenhasDivergentes_NaoDeveIniciarTransacao()
    {
        var dto = CriarDto() with { ConfirmacaoSenha = "Outra@123" };

        var resultado = await _useCase.CriarAsync(dto);

        Assert.False(resultado.IsValid);
        _unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _funcionarioRepositorio.Verify(r => r.CriarAsync(It.IsAny<Funcionario>(), It.IsAny<CancellationToken>()), Times.Never);
        _segurancaRepositorio.Verify(r => r.SalvarSenhaAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_FalhaAoSalvarSenha_DeveFazerRollbackENaoComitar()
    {
        _funcionarioRepositorio
            .Setup(r => r.CriarAsync(It.IsAny<Funcionario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _segurancaRepositorio
            .Setup(r => r.SalvarSenhaAsync(10, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha simulada."));

        var resultado = await _useCase.CriarAsync(CriarDto());

        Assert.False(resultado.IsValid);
        _unitOfWork.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _funcionarioRepositorio.Verify(r => r.DeletarAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CriarFuncionarioDto CriarDto() => new(
        Nome: "Maria Silva",
        Contato: "11999990000",
        CPF: "52998224725",
        Cargo: ECargoFuncionario.Mecanico,
        ValorHora: 120,
        Senha: "Senha@123",
        ConfirmacaoSenha: "Senha@123");
}
