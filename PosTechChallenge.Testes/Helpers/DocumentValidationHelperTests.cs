using Microsoft.VisualStudio.TestTools.UnitTesting;
using PosTechChallenge.Aplicacao.Helpers;

namespace PosTechChallenge.Testes.Helpers;

[TestClass]
public class DocumentValidationHelperTests
{
    /// <summary>
    /// Testa validação com CPF válido (sem CNPJ).
    /// Esperado: null (válido)
    /// </summary>
    [TestMethod]
    public void ValidateDocument_WithValidCpf_ReturnsNull()
    {
        // Arrange
        var cpf = "11144477735"; // CPF válido (exemplo)
        var cnpj = string.Empty;

        // Act
        var result = DocumentValidationHelper.ValidateDocument(cpf, cnpj);

        // Assert
        Assert.IsNull(result, "CPF válido deve retornar null");
    }

    /// <summary>
    /// Testa validação com CNPJ válido (sem CPF).
    /// Esperado: null (válido)
    /// </summary>
    [TestMethod]
    public void ValidateDocument_WithValidCnpj_ReturnsNull()
    {
        // Arrange
        var cpf = string.Empty;
        var cnpj = "11222333000181"; // CNPJ válido (exemplo)

        // Act
        var result = DocumentValidationHelper.ValidateDocument(cpf, cnpj);

        // Assert
        Assert.IsNull(result, "CNPJ válido deve retornar null");
    }

    /// <summary>
    /// Testa validação com CPF E CNPJ fornecidos.
    /// Esperado: mensagem de erro (ambos inválido)
    /// </summary>
    [TestMethod]
    public void ValidateDocument_WithBothDocuments_ReturnsError()
    {
        // Arrange
        var cpf = "11144477735";
        var cnpj = "11222333000181";

        // Act
        var result = DocumentValidationHelper.ValidateDocument(cpf, cnpj);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("apenas"), "Erro deve mencionar que apenas um deve ser fornecido");
    }

    /// <summary>
    /// Testa validação com nenhum documento fornecido.
    /// Esperado: mensagem de erro (nenhum inválido)
    /// </summary>
    [TestMethod]
    public void ValidateDocument_WithNoDocuments_ReturnsError()
    {
        // Arrange
        var cpf = string.Empty;
        var cnpj = string.Empty;

        // Act
        var result = DocumentValidationHelper.ValidateDocument(cpf, cnpj);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("obrigatório") || result.Contains("Informe"), "Erro deve indicar que documento é obrigatório");
    }

    /// <summary>
    /// Testa validação com CPF inválido.
    /// Esperado: mensagem de erro
    /// </summary>
    [TestMethod]
    public void ValidateDocument_WithInvalidCpf_ReturnsError()
    {
        // Arrange
        var cpf = "00000000000"; // CPF inválido (todos zeros)
        var cnpj = string.Empty;

        // Act
        var result = DocumentValidationHelper.ValidateDocument(cpf, cnpj);

        // Assert
        Assert.IsNotNull(result, "CPF inválido deve retornar mensagem de erro");
    }

    /// <summary>
    /// Testa validação com CNPJ inválido.
    /// Esperado: mensagem de erro
    /// </summary>
    [TestMethod]
    public void ValidateDocument_WithInvalidCnpj_ReturnsError()
    {
        // Arrange
        var cpf = string.Empty;
        var cnpj = "00000000000000"; // CNPJ inválido (todos zeros)

        // Act
        var result = DocumentValidationHelper.ValidateDocument(cpf, cnpj);

        // Assert
        Assert.IsNotNull(result, "CNPJ inválido deve retornar mensagem de erro");
    }

    /// <summary>
    /// Testa ValidateCpf com CPF válido.
    /// Esperado: null
    /// </summary>
    [TestMethod]
    public void ValidateCpf_WithValidCpf_ReturnsNull()
    {
        // Arrange
        var cpf = "11144477735";

        // Act
        var result = DocumentValidationHelper.ValidateCpf(cpf);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Testa ValidateCpf com CPF vazio.
    /// Esperado: mensagem de erro
    /// </summary>
    [TestMethod]
    public void ValidateCpf_WithEmptyCpf_ReturnsError()
    {
        // Arrange
        var cpf = string.Empty;

        // Act
        var result = DocumentValidationHelper.ValidateCpf(cpf);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("obrigatório"), "Erro deve indicar que CPF é obrigatório");
    }

    /// <summary>
    /// Testa ValidateCnpj com CNPJ válido.
    /// Esperado: null
    /// </summary>
    [TestMethod]
    public void ValidateCnpj_WithValidCnpj_ReturnsNull()
    {
        // Arrange
        var cnpj = "11222333000181";

        // Act
        var result = DocumentValidationHelper.ValidateCnpj(cnpj);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Testa ValidateCnpj com CNPJ vazio.
    /// Esperado: mensagem de erro
    /// </summary>
    [TestMethod]
    public void ValidateCnpj_WithEmptyCnpj_ReturnsError()
    {
        // Arrange
        var cnpj = string.Empty;

        // Act
        var result = DocumentValidationHelper.ValidateCnpj(cnpj);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("obrigatório"), "Erro deve indicar que CNPJ é obrigatório");
    }
}
