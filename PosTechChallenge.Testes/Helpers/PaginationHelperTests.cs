using Microsoft.VisualStudio.TestTools.UnitTesting;
using PosTechChallenge.Aplicacao.Helpers;

namespace PosTechChallenge.Testes.Helpers;

[TestClass]
public class PaginationHelperTests
{
    /// <summary>
    /// 📌 TESTE CRÍTICO: Valida se o bug de paginação foi corrigido.
    ///
    /// BUG ORIGINAL:
    /// ClienteService.cs linha 106: TotalPages = quantidadeClientes / 10
    /// Resultado: 105 itens = 10 páginas ❌ ERRADO
    ///
    /// ESPERADO APÓS CORRIGIR:
    /// PaginationHelper.CalculateTotalPages(105, 10) = 11 páginas ✅ CORRETO
    /// </summary>
    [TestMethod]
    public void CalculateTotalPages_With105ItemsAnd10PageSize_Returns11()
    {
        // Arrange
        int totalItems = 105;
        int pageSize = 10;

        // Act
        int result = PaginationHelper.CalculateTotalPages(totalItems, pageSize);

        // Assert
        Assert.AreEqual(11, result, "105 itens com pageSize 10 deve resultar em 11 páginas (não 10)");
    }

    /// <summary>
    /// Testa cálculo com número par de páginas.
    /// 100 itens / 10 = exatamente 10 páginas
    /// </summary>
    [TestMethod]
    public void CalculateTotalPages_With100ItemsAnd10PageSize_Returns10()
    {
        // Arrange
        int totalItems = 100;
        int pageSize = 10;

        // Act
        int result = PaginationHelper.CalculateTotalPages(totalItems, pageSize);

        // Assert
        Assert.AreEqual(10, result);
    }

    /// <summary>
    /// Testa com único item.
    /// 1 item sempre = 1 página
    /// </summary>
    [TestMethod]
    public void CalculateTotalPages_With1Item_Returns1()
    {
        // Arrange
        int totalItems = 1;
        int pageSize = 10;

        // Act
        int result = PaginationHelper.CalculateTotalPages(totalItems, pageSize);

        // Assert
        Assert.AreEqual(1, result);
    }

    /// <summary>
    /// Testa com zero itens.
    /// Nenhum item = 0 páginas
    /// </summary>
    [TestMethod]
    public void CalculateTotalPages_With0Items_Returns0()
    {
        // Arrange
        int totalItems = 0;
        int pageSize = 10;

        // Act
        int result = PaginationHelper.CalculateTotalPages(totalItems, pageSize);

        // Assert
        Assert.AreEqual(0, result);
    }

    /// <summary>
    /// Testa com pageSize = 1.
    /// 50 itens com pageSize 1 = 50 páginas
    /// </summary>
    [TestMethod]
    public void CalculateTotalPages_With50ItemsAnd1PageSize_Returns50()
    {
        // Arrange
        int totalItems = 50;
        int pageSize = 1;

        // Act
        int result = PaginationHelper.CalculateTotalPages(totalItems, pageSize);

        // Assert
        Assert.AreEqual(50, result);
    }

    /// <summary>
    /// Testa com pageSize grande.
    /// 10 itens com pageSize 100 = 1 página
    /// </summary>
    [TestMethod]
    public void CalculateTotalPages_With10ItemsAnd100PageSize_Returns1()
    {
        // Arrange
        int totalItems = 10;
        int pageSize = 100;

        // Act
        int result = PaginationHelper.CalculateTotalPages(totalItems, pageSize);

        // Assert
        Assert.AreEqual(1, result);
    }

    /// <summary>
    /// Testa ValidatePaginationParams com valores válidos.
    /// Esperado: null (válido)
    /// </summary>
    [TestMethod]
    public void ValidatePaginationParams_WithValidParams_ReturnsNull()
    {
        // Arrange
        int page = 1;
        int pageSize = 10;

        // Act
        var result = PaginationHelper.ValidatePaginationParams(page, pageSize);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Testa com página zero.
    /// Esperado: mensagem de erro (página deve ser > 0)
    /// </summary>
    [TestMethod]
    public void ValidatePaginationParams_WithPageZero_ReturnsError()
    {
        // Arrange
        int page = 0;
        int pageSize = 10;

        // Act
        var result = PaginationHelper.ValidatePaginationParams(page, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("maior"), "Erro deve indicar que página deve ser maior");
    }

    /// <summary>
    /// Testa com página negativa.
    /// Esperado: mensagem de erro
    /// </summary>
    [TestMethod]
    public void ValidatePaginationParams_WithNegativePage_ReturnsError()
    {
        // Arrange
        int page = -1;
        int pageSize = 10;

        // Act
        var result = PaginationHelper.ValidatePaginationParams(page, pageSize);

        // Assert
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// Testa com pageSize zero.
    /// Esperado: mensagem de erro (pageSize deve ser > 0)
    /// </summary>
    [TestMethod]
    public void ValidatePaginationParams_WithPageSizeZero_ReturnsError()
    {
        // Arrange
        int page = 1;
        int pageSize = 0;

        // Act
        var result = PaginationHelper.ValidatePaginationParams(page, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("página"), "Erro deve indicar que tamanho da página é inválido");
    }

    /// <summary>
    /// Testa CalculateOffset para query OFFSET/FETCH.
    /// Página 1, PageSize 10 = pular 0 linhas
    /// </summary>
    [TestMethod]
    public void CalculateOffset_Page1_Returns0()
    {
        // Act
        var result = PaginationHelper.CalculateOffset(1, 10);

        // Assert
        Assert.AreEqual(0, result);
    }

    /// <summary>
    /// Testa CalculateOffset para página 2.
    /// Página 2, PageSize 10 = pular 10 linhas
    /// </summary>
    [TestMethod]
    public void CalculateOffset_Page2_Returns10()
    {
        // Act
        var result = PaginationHelper.CalculateOffset(2, 10);

        // Assert
        Assert.AreEqual(10, result);
    }

    /// <summary>
    /// Testa CalculateOffset para página 3.
    /// Página 3, PageSize 10 = pular 20 linhas
    /// </summary>
    [TestMethod]
    public void CalculateOffset_Page3_Returns20()
    {
        // Act
        var result = PaginationHelper.CalculateOffset(3, 10);

        // Assert
        Assert.AreEqual(20, result);
    }

    /// <summary>
    /// Testa IsValidPage com página válida.
    /// Esperado: true
    /// </summary>
    [TestMethod]
    public void IsValidPage_WithValidPage_ReturnsTrue()
    {
        // Act
        var result = PaginationHelper.IsValidPage(1, 10);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Testa IsValidPage com página zero.
    /// Esperado: false (não existe página 0)
    /// </summary>
    [TestMethod]
    public void IsValidPage_WithPageZero_ReturnsFalse()
    {
        // Act
        var result = PaginationHelper.IsValidPage(0, 10);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Testa IsValidPage com página acima do total.
    /// Esperado: false (página não existe)
    /// </summary>
    [TestMethod]
    public void IsValidPage_WithPageAboveTotal_ReturnsFalse()
    {
        // Act
        var result = PaginationHelper.IsValidPage(15, 10);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Testa IsValidPage com última página válida.
    /// Esperado: true
    /// </summary>
    [TestMethod]
    public void IsValidPage_WithLastValidPage_ReturnsTrue()
    {
        // Act
        var result = PaginationHelper.IsValidPage(10, 10);

        // Assert
        Assert.IsTrue(result);
    }
}
