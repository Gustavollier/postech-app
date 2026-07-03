using Microsoft.VisualStudio.TestTools.UnitTesting;
using PosTechChallenge.Aplicacao.UseCases;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Testes.UseCases;

/// <summary>
/// Mock UseCase para testar interface IUseCase{TInput, TOutput}
/// </summary>
public class MockUseCaseWithOutput : IUseCase<string, int>
{
    public async Task<Resultado<int>> ExecuteAsync(string input)
    {
        await Task.Delay(10);
        return Resultado<int>.Sucesso(input.Length);
    }
}

/// <summary>
/// Mock UseCase para testar interface IUseCase{TInput}
/// </summary>
public class MockUseCaseNoOutput : IUseCase<string>
{
    public async Task<Resultado> ExecuteAsync(string input)
    {
        await Task.Delay(10);
        return input.Length > 0
            ? Resultado.Sucesso("Processado com sucesso")
            : Resultado.Falha("Input vazio");
    }
}

[TestClass]
public class IUseCaseTests
{
    /// <summary>
    /// Testa que UseCase com output implementa interface corretamente.
    /// </summary>
    [TestMethod]
    public async Task UseCase_WithOutput_ImplementsInterfaceCorrectly()
    {
        // Arrange
        var useCase = new MockUseCaseWithOutput();

        // Act
        var result = await useCase.ExecuteAsync("Hello");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(5, result.Output);
    }

    /// <summary>
    /// Testa que UseCase sem output implementa interface corretamente.
    /// </summary>
    [TestMethod]
    public async Task UseCase_NoOutput_ImplementsInterfaceCorrectly()
    {
        // Arrange
        var useCase = new MockUseCaseNoOutput();

        // Act
        var result = await useCase.ExecuteAsync("test");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid);
    }

    /// <summary>
    /// Testa que interface força implementação de ExecuteAsync.
    /// </summary>
    [TestMethod]
    public void IUseCase_DefinesExecuteAsyncMethod()
    {
        // Arrange
        var useCaseType = typeof(IUseCase<string, int>);

        // Act
        var method = useCaseType.GetMethod("ExecuteAsync");

        // Assert
        Assert.IsNotNull(method);
        Assert.IsTrue(method.IsAbstract || method.IsVirtual);
    }

    /// <summary>
    /// Testa retorno de erro do UseCase.
    /// </summary>
    [TestMethod]
    public async Task UseCase_ReturnsError_WhenInputIsEmpty()
    {
        // Arrange
        var useCase = new MockUseCaseNoOutput();

        // Act
        var result = await useCase.ExecuteAsync(string.Empty);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsValid);
    }

    /// <summary>
    /// Testa que interface padroniza contrato de todos os Use Cases.
    /// </summary>
    [TestMethod]
    public void IUseCase_StandardizesUseCaseContract()
    {
        // Arrange
        var useCaseWithOutput = typeof(MockUseCaseWithOutput);
        var useCaseNoOutput = typeof(MockUseCaseNoOutput);

        // Act
        var interfaceWithOutput = useCaseWithOutput.GetInterfaces()
            .FirstOrDefault(i => i.Name == "IUseCase`2");
        var interfaceNoOutput = useCaseNoOutput.GetInterfaces()
            .FirstOrDefault(i => i.Name == "IUseCase`1");

        // Assert
        Assert.IsNotNull(interfaceWithOutput);
        Assert.IsNotNull(interfaceNoOutput);
    }
}
