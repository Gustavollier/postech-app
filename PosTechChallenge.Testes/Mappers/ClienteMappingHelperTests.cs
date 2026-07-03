using Microsoft.VisualStudio.TestTools.UnitTesting;
using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Aplicacao.Mappers;
using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Testes.Mappers;

[TestClass]
public class ClienteMappingHelperTests
{
    private Cliente _clienteValido;

    [TestInitialize]
    public void Setup()
    {
        _clienteValido = new Cliente
        {
            Id = 1,
            NomeCompleto = "João Silva",
            CPF = "12345678901",
            CNPJ = null,
            Telefone = "11999999999",
            Email = "joao@example.com",
            Ativo = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [TestMethod]
    public void MapEntityToDto_WithValidCliente_ReturnsMappedDto()
    {
        var result = ClienteMappingHelper.MapEntityToDto(_clienteValido);

        Assert.IsNotNull(result);
        Assert.AreEqual(_clienteValido.Id, result.Id);
        Assert.AreEqual(_clienteValido.NomeCompleto, result.NomeCompleto);
        Assert.AreEqual(_clienteValido.CPF, result.CPF);
        Assert.AreEqual(_clienteValido.Email, result.Email);
        Assert.AreEqual(_clienteValido.Ativo, result.Ativo);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MapEntityToDto_WithNullCliente_ThrowsException()
    {
        ClienteMappingHelper.MapEntityToDto(null!);
    }

    [TestMethod]
    public void MapEntityToDto_PreservesAllFields()
    {
        var dto = ClienteMappingHelper.MapEntityToDto(_clienteValido);

        Assert.AreEqual(_clienteValido.Id, dto.Id);
        Assert.AreEqual(_clienteValido.NomeCompleto, dto.NomeCompleto);
        Assert.AreEqual(_clienteValido.CPF, dto.CPF);
        Assert.AreEqual(_clienteValido.CNPJ, dto.CNPJ);
        Assert.AreEqual(_clienteValido.Telefone, dto.Telefone);
        Assert.AreEqual(_clienteValido.Email, dto.Email);
        Assert.AreEqual(_clienteValido.Ativo, dto.Ativo);
        Assert.AreEqual(_clienteValido.CreatedAt, dto.CreatedAt);
        Assert.AreEqual(_clienteValido.UpdatedAt, dto.UpdatedAt);
    }
}
