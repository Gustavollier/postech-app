using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public sealed class ItemsRepositorio : IItemsRepositorio
    {
        private readonly IDbSession _session;

        public ItemsRepositorio(IDbSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<IEnumerable<ItemOS>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                QueryItens.OBTER_TODOS,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<ItemOS>(command).ConfigureAwait(false);
        }

        public async Task<ItemOS?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                QueryItens.OBTER_POR_ID,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<ItemOS>(command).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ItemOS>> ObterPorOrdemServicoIdAsync(int ordemServicoId, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                QueryItens.OBTER_POR_ORDEM_SERVICO_ID,
                new { IdOS = ordemServicoId },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<ItemOS>(command).ConfigureAwait(false);
        }

        public async Task<int> CriarAsync(ItemOS item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                QueryItens.CRIAR,
                MapearParametros(item),
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
        }

        public async Task<bool> AtualizarAsync(ItemOS item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                QueryItens.ATUALIZAR,
                MapearParametros(item),
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }

        public async Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                QueryItens.DELETAR,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }

        private static object MapearParametros(ItemOS item)
        {
            return new
            {
                item.Id,
                item.IdOS,
                item.TipoItem,
                item.QuantidadeItem,
                IdFuncionario = item.IdFuncionario > 0 ? item.IdFuncionario : (int?)null,
                IdPeca = item.IdPeca > 0 ? item.IdPeca : (int?)null
            };
        }
    }
}
