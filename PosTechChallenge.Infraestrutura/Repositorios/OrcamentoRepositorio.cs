using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios;

public sealed class OrcamentoRepositorio : IOrcamentoRepositorio
{
    private readonly IDbSession _session;

    public OrcamentoRepositorio(IDbSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<Orcamento?> ObterPorOrdemServicoIdAsync(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            OrcamentoQuerys.OBTER_POR_ORDEM_SERVICO_ID,
            new { IdOS = ordemServicoId },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<Orcamento>(command).ConfigureAwait(false);
    }

    public async Task<OrcamentoValores> CalcularValoresAsync(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            OrcamentoQuerys.CALCULAR_VALORES,
            new { IdOS = ordemServicoId },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<OrcamentoValores>(command).ConfigureAwait(false);
    }

    public async Task<int> CriarAsync(Orcamento orcamento, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orcamento);

        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            OrcamentoQuerys.CRIAR,
            orcamento,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
    }

    public async Task<bool> AtualizarAsync(Orcamento orcamento, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orcamento);

        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            OrcamentoQuerys.ATUALIZAR,
            orcamento,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
        return linhasAfetadas > 0;
    }
}
