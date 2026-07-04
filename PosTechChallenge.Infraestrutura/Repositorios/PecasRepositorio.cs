using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public sealed class PecasRepositorio : IPecasRepositorio
    {
        private readonly IDbSession _session;

        public PecasRepositorio(IDbSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<IEnumerable<Pecas>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                PecasQuerys.OBTER_TODOS,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Pecas>(command).ConfigureAwait(false);
        }

        public async Task<Pecas?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                PecasQuerys.OBTER_POR_ID,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Pecas>(command).ConfigureAwait(false);
        }

        public async Task<int> CriarAsync(Pecas peca, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(peca);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                PecasQuerys.CRIAR,
                peca,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
        }

        public async Task<bool> AtualizarAsync(Pecas peca, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(peca);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                PecasQuerys.ATUALIZAR,
                peca,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }

        public async Task<bool> AjustarEstoqueAsync(int id, int quantidadeEstoque, DateTime atualizadoEm, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                PecasQuerys.AJUSTAR_ESTOQUE,
                new { Id = id, QuantidadeEstoque = quantidadeEstoque, AtualizadoEm = atualizadoEm },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }

        public async Task<bool> DeletarAsync(int id, DateTime atualizadoEm, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                PecasQuerys.DELETAR,
                new { Id = id, AtualizadoEm = atualizadoEm },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }
    }
}
