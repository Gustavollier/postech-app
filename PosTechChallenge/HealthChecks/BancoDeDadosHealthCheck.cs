using System.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PosTechChallenge.Infraestrutura.Data;

namespace PosTechChallenge.HealthChecks;

/// <summary>
/// Verifica se o banco responde, usando a <see cref="IDbSession"/> do próprio escopo
/// da requisição. Aproveitar a sessão existente evita abrir uma segunda conexão e
/// garante que o health check exercita exatamente a mesma connection string que a
/// aplicação usa em produção.
/// </summary>
public sealed class BancoDeDadosHealthCheck : IHealthCheck
{
    private readonly IDbSession _dbSession;

    public BancoDeDadosHealthCheck(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _dbSession.GetConnectionAsync(cancellationToken).ConfigureAwait(false);

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandType = CommandType.Text;

            await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            return HealthCheckResult.Healthy("Banco de dados respondendo.");
        }
        catch (Exception excecao)
        {
            // Degradado, não insalubre: o processo está vivo e serve as rotas que
            // não dependem do banco. Quem decide tirar o pod do balanceador é o
            // readiness probe, que consulta apenas esta tag.
            return HealthCheckResult.Unhealthy("Banco de dados inacessível.", excecao);
        }
    }
}
