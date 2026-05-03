using Moq;
using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Aplicacao.UseCases.Funcionario;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using Xunit;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Testes.Application;

public class CriarFuncionarioUseCaseTests
{
    private readonly Mock<IFuncionarioRepositorio> _funcionarioRepositorio = new();
    private readonly Mock<ISegurancaRepositorio> _segurancaRepositorio = new();
    private readonly CriarFuncionarioUseCase _useCase;

    public CriarFuncionarioUseCaseTests()
    {
        _useCase = new CriarFuncionarioUseCase(
            _funcionarioRepositorio.Object,
            _segurancaRepositorio.Object);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarFuncionarioESenha()
    {
        string? senhaHash = null;

        _funcionarioRepositorio
            .Setup(r => r.CriarAsync(It.IsAny<Funcionario>()))
            .ReturnsAsync(10);

        _segurancaRepositorio
            .Setup(r => r.CriarSenhaAsync(10, It.IsAny<string>()))
            .Callback<int, string>((_, hash) => senhaHash = hash)
            .Returns(Task.CompletedTask);

        var resultado = await _useCase.CriarAsync(CriarDto());

        Assert.True(resultado.IsValid);
        Assert.NotNull(senhaHash);
        Assert.True(PasswordHasher.VerifyPassword("Senha@123", senhaHash!));
    }

    [Fact]
    public async Task CriarAsync_SenhasDivergentes_NaoDevePersistirFuncionario()
    {
        var dto = CriarDto() with { ConfirmacaoSenha = "Outra@123" };

        var resultado = await _useCase.CriarAsync(dto);

        Assert.False(resultado.IsValid);
        _funcionarioRepositorio.Verify(r => r.CriarAsync(It.IsAny<Funcionario>()), Times.Never);
        _segurancaRepositorio.Verify(r => r.CriarSenhaAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_FalhaAoCriarSenha_DeveRemoverFuncionarioCriado()
    {
        _funcionarioRepositorio
            .Setup(r => r.CriarAsync(It.IsAny<Funcionario>()))
            .ReturnsAsync(10);

        _segurancaRepositorio
            .Setup(r => r.CriarSenhaAsync(10, It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Falha simulada."));

        var resultado = await _useCase.CriarAsync(CriarDto());

        Assert.False(resultado.IsValid);
        _funcionarioRepositorio.Verify(r => r.DeletarAsync(10), Times.Once);
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
