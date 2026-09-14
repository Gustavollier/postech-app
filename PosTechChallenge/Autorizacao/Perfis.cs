using System.Security.Claims;

namespace PosTechChallenge.Autorizacao;

/// <summary>
/// Perfis de acesso da API.
///
/// Existem dois emissores de token, ambos assinados com o mesmo segredo e
/// aceitos pelo mesmo validate-jwt do gateway:
///
/// - a própria API, no login de funcionário, que coloca o cargo em
///   <see cref="ClaimTypes.Role"/>;
/// - a função de autenticação por CPF, que coloca "Cliente" em
///   <see cref="ClaimTypes.Role"/> e o id do cliente em "ClienteId".
///
/// Sem distinguir os dois, "estar autenticado" bastava para ler os dados
/// pessoais de todos os clientes e as ordens de toda a oficina — um token de
/// cliente valia tanto quanto um de gerente nas rotas de leitura.
/// </summary>
public static class Perfis
{
    /// <summary>Papel que a função de autenticação por CPF emite.</summary>
    public const string Cliente = "Cliente";

    /// <summary>Nome da policy que exige um cargo de funcionário.</summary>
    public const string Equipe = "Equipe";

    /// <summary>Cargos do enum de Funcionario, como o token os emite.</summary>
    public static readonly string[] Cargos = ["Gerente", "Recepcionista", "Mecanico", "Estoquista"];
}

public static class PerfilExtensions
{
    public static bool EhCliente(this ClaimsPrincipal usuario) =>
        usuario.IsInRole(Perfis.Cliente);

    /// <summary>Id do cliente dono do token, quando o token é de cliente.</summary>
    public static int? ObterClienteId(this ClaimsPrincipal usuario) =>
        int.TryParse(usuario.FindFirst("ClienteId")?.Value, out var id) ? id : null;

    /// <summary>
    /// Verdadeiro quando quem chama é um cliente pedindo dado de outro cliente.
    ///
    /// Um funcionário nunca cai aqui: a checagem existe só para o perfil que
    /// tem escopo restrito ao próprio cadastro.
    /// </summary>
    public static bool ClienteAcessandoOutro(this ClaimsPrincipal usuario, int idCliente) =>
        usuario.EhCliente() && usuario.ObterClienteId() != idCliente;
}
