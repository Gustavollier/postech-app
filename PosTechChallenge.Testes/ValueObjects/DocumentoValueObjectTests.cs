using Microsoft.VisualStudio.TestTools.UnitTesting;
using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Testes.ValueObjects;

[TestClass]
public class DocumentoValueObjectTests
{
    /// <summary>
    /// Testa criação de DocumentoValueObject com CPF válido.
    /// </summary>
    [TestMethod]
    public void CriarComCpf_WithValidCpf_CreatesSuccessfully()
    {
        // Arrange
        var cpf = "11144477735";

        // Act
        var documento = DocumentoValueObject.CriarComCpf(cpf);

        // Assert
        Assert.IsNotNull(documento);
        Assert.AreEqual(cpf, documento.Valor);
        Assert.AreEqual(DocumentoValueObject.TipoDocumento.CPF, documento.Tipo);
        Assert.IsTrue(documento.IsCpf);
        Assert.IsFalse(documento.IsCnpj);
    }

    /// <summary>
    /// Testa criação com CPF vazio.
    /// Esperado: ArgumentException
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CriarComCpf_WithEmptyCpf_ThrowsException()
    {
        // Act
        DocumentoValueObject.CriarComCpf(string.Empty);
    }

    /// <summary>
    /// Testa criação com CPF inválido.
    /// Esperado: ArgumentException (validação do CpfValueObject)
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CriarComCpf_WithInvalidCpf_ThrowsException()
    {
        // Act
        DocumentoValueObject.CriarComCpf("00000000000");
    }

    /// <summary>
    /// Testa criação de DocumentoValueObject com CNPJ válido.
    /// </summary>
    [TestMethod]
    public void CriarComCnpj_WithValidCnpj_CreatesSuccessfully()
    {
        // Arrange
        var cnpj = "11222333000181";

        // Act
        var documento = DocumentoValueObject.CriarComCnpj(cnpj);

        // Assert
        Assert.IsNotNull(documento);
        Assert.AreEqual(cnpj, documento.Valor);
        Assert.AreEqual(DocumentoValueObject.TipoDocumento.CNPJ, documento.Tipo);
        Assert.IsFalse(documento.IsCpf);
        Assert.IsTrue(documento.IsCnpj);
    }

    /// <summary>
    /// Testa criação com CNPJ vazio.
    /// Esperado: ArgumentException
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CriarComCnpj_WithEmptyCnpj_ThrowsException()
    {
        // Act
        DocumentoValueObject.CriarComCnpj(string.Empty);
    }

    /// <summary>
    /// Testa criação com CNPJ inválido.
    /// Esperado: ArgumentException (validação do CnpjValueObject)
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CriarComCnpj_WithInvalidCnpj_ThrowsException()
    {
        // Act
        DocumentoValueObject.CriarComCnpj("00000000000000");
    }

    /// <summary>
    /// Testa factory method Criar() com CPF válido.
    /// </summary>
    [TestMethod]
    public void Criar_WithValidCpf_CreatesCpfDocumento()
    {
        // Act
        var documento = DocumentoValueObject.Criar("11144477735", null);

        // Assert
        Assert.IsTrue(documento.IsCpf);
        Assert.AreEqual("11144477735", documento.ObterCpf());
        Assert.IsNull(documento.ObterCnpj());
    }

    /// <summary>
    /// Testa factory method Criar() com CNPJ válido.
    /// </summary>
    [TestMethod]
    public void Criar_WithValidCnpj_CreatesCnpjDocumento()
    {
        // Act
        var documento = DocumentoValueObject.Criar(null, "11222333000181");

        // Assert
        Assert.IsTrue(documento.IsCnpj);
        Assert.IsNull(documento.ObterCpf());
        Assert.AreEqual("11222333000181", documento.ObterCnpj());
    }

    /// <summary>
    /// REGRA CRÍTICA: Testa que Criar() com ambos os documentos lança erro.
    /// Nunca permitir Cliente com CPF e CNPJ simultaneamente!
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Criar_WithBothCpfAndCnpj_ThrowsException()
    {
        // Act - Esta ação deve ser IMPOSSÍVEL
        DocumentoValueObject.Criar("11144477735", "11222333000181");
    }

    /// <summary>
    /// REGRA CRÍTICA: Testa que Criar() com nenhum documento lança erro.
    /// Nunca permitir Cliente sem documento!
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Criar_WithNeitherCpfNorCnpj_ThrowsException()
    {
        // Act - Esta ação deve ser IMPOSSÍVEL
        DocumentoValueObject.Criar(null, null);
    }

    /// <summary>
    /// Testa igualdade entre DocumentoValueObjects.
    /// Value Objects com mesmo valor e tipo devem ser iguais.
    /// </summary>
    [TestMethod]
    public void Equality_WithSameValues_AreEqual()
    {
        // Arrange
        var documento1 = DocumentoValueObject.CriarComCpf("11144477735");
        var documento2 = DocumentoValueObject.CriarComCpf("11144477735");

        // Act & Assert
        Assert.AreEqual(documento1, documento2);
        Assert.IsTrue(documento1 == documento2);
        Assert.IsFalse(documento1 != documento2);
    }

    /// <summary>
    /// Testa desigualdade entre DocumentoValueObjects.
    /// Value Objects com valores diferentes devem ser diferentes.
    /// </summary>
    [TestMethod]
    public void Equality_WithDifferentValues_AreNotEqual()
    {
        // Arrange
        var documento1 = DocumentoValueObject.CriarComCpf("11144477735");
        var documento2 = DocumentoValueObject.CriarComCpf("52998224725");

        // Act & Assert
        Assert.AreNotEqual(documento1, documento2);
        Assert.IsTrue(documento1 != documento2);
        Assert.IsFalse(documento1 == documento2);
    }

    /// <summary>
    /// Testa desigualdade entre tipos diferentes.
    /// CPF e CNPJ são diferentes mesmo se o número fosse o mesmo.
    /// </summary>
    [TestMethod]
    public void Equality_WithDifferentTypes_AreNotEqual()
    {
        // Arrange
        var cpf = DocumentoValueObject.CriarComCpf("11144477735");
        var cnpj = DocumentoValueObject.CriarComCnpj("11222333000181");

        // Act & Assert
        Assert.AreNotEqual(cpf, cnpj);
    }

    /// <summary>
    /// Testa ToString() para CPF.
    /// </summary>
    [TestMethod]
    public void ToString_WithCpf_ReturnsFormattedString()
    {
        // Arrange
        var documento = DocumentoValueObject.CriarComCpf("11144477735");

        // Act
        var resultado = documento.ToString();

        // Assert
        Assert.IsTrue(resultado.Contains("CPF"));
        Assert.IsTrue(resultado.Contains("11144477735"));
    }

    /// <summary>
    /// Testa ToString() para CNPJ.
    /// </summary>
    [TestMethod]
    public void ToString_WithCnpj_ReturnsFormattedString()
    {
        // Arrange
        var documento = DocumentoValueObject.CriarComCnpj("11222333000181");

        // Act
        var resultado = documento.ToString();

        // Assert
        Assert.IsTrue(resultado.Contains("CNPJ"));
        Assert.IsTrue(resultado.Contains("11222333000181"));
    }

    /// <summary>
    /// Testa que Value Objects são imutáveis.
    /// Após criação, valores não podem ser alterados.
    /// </summary>
    [TestMethod]
    public void ValueObject_IsImmutable()
    {
        // Arrange
        var documento = DocumentoValueObject.CriarComCpf("11144477735");

        // Act & Assert - Valor e Tipo não têm setters
        Assert.AreEqual("11144477735", documento.Valor);
        Assert.AreEqual(DocumentoValueObject.TipoDocumento.CPF, documento.Tipo);

        // Se tentássemos documento.Valor = "novo", receberia erro de compilação
        // Isso é testado em tempo de compilação, não em runtime
    }

    /// <summary>
    /// Testa hash code consistency.
    /// Mesmo objeto deve ter mesmo hash code.
    /// </summary>
    [TestMethod]
    public void GetHashCode_WithSameValues_ReturnsSameHashCode()
    {
        // Arrange
        var documento1 = DocumentoValueObject.CriarComCpf("11144477735");
        var documento2 = DocumentoValueObject.CriarComCpf("11144477735");

        // Act
        var hash1 = documento1.GetHashCode();
        var hash2 = documento2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    /// <summary>
    /// Teste de cenário real: Garantir que é IMPOSSÍVEL criar Cliente inválido.
    /// </summary>
    [TestMethod]
    public void RealWorldScenario_MultipleValidationsEnsureInvariants()
    {
        // ✅ POSSÍVEL: Criar com CPF
        var doc1 = DocumentoValueObject.Criar("11144477735", null);
        Assert.IsTrue(doc1.IsCpf);

        // ✅ POSSÍVEL: Criar com CNPJ
        var doc2 = DocumentoValueObject.Criar(null, "11222333000181");
        Assert.IsTrue(doc2.IsCnpj);

        // ❌ IMPOSSÍVEL: Criar sem documento
        try
        {
            DocumentoValueObject.Criar(null, null);
            Assert.Fail("Deveria ter lançado exceção");
        }
        catch (ArgumentException ex)
        {
            Assert.IsTrue(ex.Message.Contains("obrigatório"));
        }

        // ❌ IMPOSSÍVEL: Criar com ambos
        try
        {
            DocumentoValueObject.Criar("11144477735", "11222333000181");
            Assert.Fail("Deveria ter lançado exceção");
        }
        catch (ArgumentException ex)
        {
            Assert.IsTrue(ex.Message.Contains("apenas"));
        }
    }
}
