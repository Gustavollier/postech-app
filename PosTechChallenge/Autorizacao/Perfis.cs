using System.Security.Claims;
using PosTechChallenge.Aplicacao.Interface.Services;
using static PosTechChallenge.Dominio.Utils.Enums;

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

    /// <summary>
    /// Cargos de funcionario, como o token os emite.
    ///
    /// Derivado do enum, e nao escrito a mao: a lista fixa tinha so quatro dos
    /// sete cargos, entao Eletricista, Lavador e Supervisor eram recusados em
    /// toda rota de operacao — e a tela de cadastro oferece os sete.
    /// </summary>
    public static readonly string[] Cargos = Enum.GetNames<ECargoFuncionario>();
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

    /// <summary>
    /// Posse de uma parte da ordem — orcamento, item, valor, historico.
    ///
    /// Nenhum desses registros carrega o id do cliente: todos conhecem so o
    /// IdOS. Quem sabe de quem e a ordem e a propria ordem, entao a checagem
    /// passa por ela.
    ///
    /// Um funcionario nunca chega a consultar: a pergunta so faz sentido para o
    /// perfil com escopo restrito, e sair antes evita uma ida ao banco por
    /// requisicao. Ordem inexistente tambem nao e problema de posse — quem
    /// responde por isso e a propria rota, com 404.
    /// </summary>
    public static async Task<bool> ClienteAcessandoOrdemDeOutroAsync(
        this ClaimsPrincipal usuario,
        IOrdemServicoService ordens,
        int idOS)
    {
        if (!usuario.EhCliente())
            return false;

        var resultado = await ordens.ObterPorIdAsync(idOS);

        if (resultado is null || !resultado.IsValid || resultado.Output is null)
            return false;

        return usuario.ClienteAcessandoOutro(resultado.Output.IdCliente);
    }
}
