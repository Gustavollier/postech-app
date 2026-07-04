using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public sealed class StatusRepositorio : IStatusRepositorio
    {
        private readonly IDbSession _session;

        public StatusRepositorio(IDbSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<IEnumerable<Status>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                StatusQuerys.OBTER_TODOS,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Status>(command).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Status>> ObterPorOrdemServicoIdAsync(int ordemServicoId, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                StatusQuerys.OBTER_POR_ORDEM_SERVICO_ID,
                new { IdOS = ordemServicoId },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Status>(command).ConfigureAwait(false);
        }

        public async Task<Status?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                StatusQuerys.OBTER_POR_ID,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Status>(command).ConfigureAwait(false);
        }

        public async Task<int> CriarAsync(Status status, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(status);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                StatusQuerys.CRIAR,
                status,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
        }

        public async Task<bool> AtualizarAsync(Status status, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(status);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                StatusQuerys.ATUALIZAR,
                status,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }
    }
}
