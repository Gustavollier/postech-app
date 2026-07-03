using Microsoft.VisualStudio.TestTools.UnitTesting;
using PosTechChallenge.Aplicacao.Dto.Common;

namespace PosTechChallenge.Testes.Dto.Common;

[TestClass]
public class PaginatedResponseDtoTests
{
    /// <summary>
    /// Testa criação de resposta paginada genérica.
    /// Verifica se propriedades são atribuídas corretamente.
    /// </summary>
    [TestMethod]
    public void PaginatedResponseDto_Generic_CreatedSuccessfully()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var dto = new PaginatedResponseDto<int>
        {
            Items = items,
            Page = 1,
            PageSize = 10,
            TotalItems = 25,
            TotalPages = 3
        };

        // Act & Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(1, dto.Page);
        Assert.AreEqual(10, dto.PageSize);
        Assert.AreEqual(25, dto.TotalItems);
        Assert.AreEqual(3, dto.TotalPages);
        Assert.AreEqual(5, dto.Items?.Count());
    }

    /// <summary>
    /// Testa propriedade HasNextPage na primeira página.
    /// Página 1 de 3 = true (tem próxima)
    /// </summary>
    [TestMethod]
    public void HasNextPage_OnFirstPageOfThree_ReturnsTrue()
    {
        // Arrange
        var dto = new PaginatedResponseDto<string>
        {
            Page = 1,
            TotalPages = 3
        };

        // Act & Assert
        Assert.IsTrue(dto.HasNextPage);
    }

    /// <summary>
    /// Testa HasNextPage na última página.
    /// Página 3 de 3 = false (não tem próxima)
    /// </summary>
    [TestMethod]
    public void HasNextPage_OnLastPage_ReturnsFalse()
    {
        // Arrange
        var dto = new PaginatedResponseDto<string>
        {
            Page = 3,
            TotalPages = 3
        };

        // Act & Assert
        Assert.IsFalse(dto.HasNextPage);
    }

    /// <summary>
    /// Testa HasPreviousPage na primeira página.
    /// Página 1 = false (não tem anterior)
    /// </summary>
    [TestMethod]
    public void HasPreviousPage_OnFirstPage_ReturnsFalse()
    {
        // Arrange
        var dto = new PaginatedResponseDto<string>
        {
            Page = 1,
            TotalPages = 3
        };

        // Act & Assert
        Assert.IsFalse(dto.HasPreviousPage);
    }

    /// <summary>
    /// Testa HasPreviousPage na página do meio.
    /// Página 2 = true (tem anterior)
    /// </summary>
    [TestMethod]
    public void HasPreviousPage_OnMiddlePage_ReturnsTrue()
    {
        // Arrange
        var dto = new PaginatedResponseDto<string>
        {
            Page = 2,
            TotalPages = 3
        };

        // Act & Assert
        Assert.IsTrue(dto.HasPreviousPage);
    }

    /// <summary>
    /// Testa página única.
    /// Página 1 de 1: sem anterior, sem próxima
    /// </summary>
    [TestMethod]
    public void SinglePage_HasNoPreviousAndNoNext()
    {
        // Arrange
        var dto = new PaginatedResponseDto<string>
        {
            Page = 1,
            TotalPages = 1
        };

        // Act & Assert
        Assert.IsFalse(dto.HasPreviousPage);
        Assert.IsFalse(dto.HasNextPage);
    }

    /// <summary>
    /// Testa com objeto genérico.
    /// Verifica type-safety
    /// </summary>
    [TestMethod]
    public void PaginatedResponseDto_WithCustomType_WorksCorrectly()
    {
        // Arrange
        var items = new[]
        {
            new { Id = 1, Name = "Item 1" },
            new { Id = 2, Name = "Item 2" }
        };

        var dto = new PaginatedResponseDto<object>
        {
            Items = items.Cast<object>(),
            Page = 1,
            PageSize = 10,
            TotalItems = 2,
            TotalPages = 1
        };

        // Act & Assert
        Assert.IsNotNull(dto.Items);
        Assert.AreEqual(2, dto.Items.Count());
    }

    /// <summary>
    /// Testa DTO não-genérico.
    /// Verifica compatibilidade
    /// </summary>
    [TestMethod]
    public void PaginatedResponseDto_NonGeneric_WorksCorrectly()
    {
        // Arrange
        var items = new object[] { "Item1", "Item2", "Item3" };
        var dto = new PaginatedResponseDto
        {
            Items = items,
            Page = 1,
            PageSize = 10,
            TotalItems = 3,
            TotalPages = 1
        };

        // Act & Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(3, dto.Items?.Count());
    }

    /// <summary>
    /// Testa com Items null.
    /// Esperado: null é válido (pode ser preenchido depois)
    /// </summary>
    [TestMethod]
    public void PaginatedResponseDto_WithNullItems_IsValid()
    {
        // Arrange
        var dto = new PaginatedResponseDto<string>
        {
            Items = null,
            Page = 1,
            PageSize = 10,
            TotalItems = 0,
            TotalPages = 0
        };

        // Act & Assert
        Assert.IsNull(dto.Items);
        Assert.AreEqual(1, dto.Page);
    }

    /// <summary>
    /// Testa cenário real: página 2 de 11 com 105 itens total.
    /// Simula resposta após correção de paginação
    /// </summary>
    [TestMethod]
    public void PaginatedResponseDto_RealWorldScenario_105ItemsPageSize10()
    {
        // Arrange: 105 itens, pageSize 10 = 11 páginas (após corrigir bug)
        var items = Enumerable.Range(11, 10); // Items 11-20 (página 2)

        var dto = new PaginatedResponseDto<int>
        {
            Items = items,
            Page = 2,
            PageSize = 10,
            TotalItems = 105,
            TotalPages = 11 // CORRIGIDO: era 10 antes
        };

        // Act & Assert
        Assert.AreEqual(2, dto.Page);
        Assert.AreEqual(10, dto.Items?.Count());
        Assert.AreEqual(11, dto.TotalPages);
        Assert.IsTrue(dto.HasNextPage); // Página 2 de 11 tem próxima
        Assert.IsTrue(dto.HasPreviousPage); // Página 2 tem anterior
    }
}
