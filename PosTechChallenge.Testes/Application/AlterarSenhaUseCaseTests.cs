using Moq;
using PosTechChallenge.Aplicacao.UseCases.Autenticacao;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using Xunit;

namespace PosTechChallenge.Testes.Application;

public class AlterarSenhaUseCaseTests
{
    private readonly Mock<ISegurancaRepositorio> _segurancaRepositorio = new();
    private readonly AlterarSenhaUseCase _useCase;

    public AlterarSenhaUseCaseTests()
    {
        _useCase = new AlterarSenhaUseCase(_segurancaRepositorio.Object);
    }

    [Fact]
    public async Task AlterarSenhaAsync_DadosValidos_DeveAtualizarHash()
    {
        string? novoHash = null;
        var hashAtual = PasswordHasher.HashPassword("Senha@123");

        _segurancaRepositorio
            .Setup(r => r.ObterPorFuncionarioIdAsync(1))
            .ReturnsAsync(new Seguranca { FuncionarioId = 1, SenhaHash = hashAtual });

        _segurancaRepositorio
            .Setup(r => r.SalvarSenhaAsync(1, It.IsAny<string>()))
            .Callback<int, string>((_, hash) => novoHash = hash)
            .Returns(Task.CompletedTask);

        var resultado = await _useCase.AlterarSenhaAsync(1, "Senha@123", "NovaSenha@123", "NovaSenha@123");

        Assert.True(resultado.IsValid);
        Assert.NotNull(novoHash);
        Assert.True(PasswordHasher.VerifyPassword("NovaSenha@123", novoHash!));
    }

    [Fact]
    public async Task AlterarSenhaAsync_SenhaAtualInvalida_NaoDeveAtualizarHash()
    {
        var hashAtual = PasswordHasher.HashPassword("Senha@123");

        _segurancaRepositorio
            .Setup(r => r.ObterPorFuncionarioIdAsync(1))
            .ReturnsAsync(new Seguranca { FuncionarioId = 1, SenhaHash = hashAtual });

        var resultado = await _useCase.AlterarSenhaAsync(1, "SenhaErrada@123", "NovaSenha@123", "NovaSenha@123");

        Assert.False(resultado.IsValid);
        _segurancaRepositorio.Verify(r => r.SalvarSenhaAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AlterarSenhaAsync_SenhasDivergentes_NaoDeveAtualizarHash()
    {
        var hashAtual = PasswordHasher.HashPassword("Senha@123");

        _segurancaRepositorio
            .Setup(r => r.ObterPorFuncionarioIdAsync(1))
            .ReturnsAsync(new Seguranca { FuncionarioId = 1, SenhaHash = hashAtual });

        var resultado = await _useCase.AlterarSenhaAsync(1, "Senha@123", "NovaSenha@123", "OutraSenha@123");

        Assert.False(resultado.IsValid);
        _segurancaRepositorio.Verify(r => r.SalvarSenhaAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}
