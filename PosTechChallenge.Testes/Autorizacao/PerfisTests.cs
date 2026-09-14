using System.Security.Claims;
using PosTechChallenge.Autorizacao;
using Xunit;

namespace PosTechChallenge.Testes.Autorizacao;

/// <summary>
/// Regra de escopo do perfil Cliente.
///
/// Estar autenticado não é o mesmo que ter acesso: o token emitido pela função
/// de autenticação por CPF é válido para a API inteira, então quem decide o que
/// ele alcança é esta regra. Um erro aqui reabre o acesso aos dados de todos os
/// clientes.
/// </summary>
public class PerfisTests
{
    private static ClaimsPrincipal Cliente(int clienteId) =>
        new(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Role, Perfis.Cliente),
                new Claim("ClienteId", clienteId.ToString()),
            ],
            authenticationType: "Teste"));

    private static ClaimsPrincipal Funcionario(string cargo = "Gerente") =>
        new(new ClaimsIdentity(
            [new Claim(ClaimTypes.Role, cargo)],
            authenticationType: "Teste"));

    [Fact]
    public void EhCliente_TokenDeCliente_DeveSerVerdadeiro()
    {
        Assert.True(Cliente(3).EhCliente());
    }

    [Theory]
    [InlineData("Gerente")]
    [InlineData("Recepcionista")]
    [InlineData("Mecanico")]
    [InlineData("Estoquista")]
    public void EhCliente_TokenDeFuncionario_DeveSerFalso(string cargo)
    {
        Assert.False(Funcionario(cargo).EhCliente());
    }

    [Fact]
    public void ObterClienteId_TokenDeCliente_DeveLerOClaim()
    {
        Assert.Equal(7, Cliente(7).ObterClienteId());
    }

    [Fact]
    public void ObterClienteId_TokenSemOClaim_DeveSerNulo()
    {
        Assert.Null(Funcionario().ObterClienteId());
    }

    [Fact]
    public void ClienteAcessandoOutro_ProprioCadastro_DevePermitir()
    {
        Assert.False(Cliente(3).ClienteAcessandoOutro(3));
    }

    [Fact]
    public void ClienteAcessandoOutro_CadastroDeTerceiro_DeveBloquear()
    {
        Assert.True(Cliente(3).ClienteAcessandoOutro(2));
    }

    [Fact]
    public void ClienteAcessandoOutro_Funcionario_NuncaEBloqueado()
    {
        // A checagem de posse existe só para o perfil de escopo restrito; a
        // equipe atende qualquer cliente e não pode ser barrada por ela.
        Assert.False(Funcionario().ClienteAcessandoOutro(2));
        Assert.False(Funcionario("Recepcionista").ClienteAcessandoOutro(99));
    }

    [Fact]
    public void ClienteAcessandoOutro_TokenDeClienteSemClienteId_DeveBloquear()
    {
        // Token malformado — papel de cliente sem dizer qual cliente. Sem o
        // claim, ObterClienteId devolve null, que nunca casa com um id real.
        var semId = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Role, Perfis.Cliente)],
            authenticationType: "Teste"));

        Assert.True(semId.ClienteAcessandoOutro(3));
    }

    [Fact]
    public void Cargos_NaoDeveIncluirOPerfilCliente()
    {
        // Se "Cliente" entrasse na lista de cargos, a policy de equipe passaria
        // a aceitar o token de cliente e a correção perderia o efeito.
        Assert.DoesNotContain(Perfis.Cliente, Perfis.Cargos);
    }
}
