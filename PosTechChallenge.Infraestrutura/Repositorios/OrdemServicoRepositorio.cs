using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public sealed class OrdemServicoRepositorio : IOrdemServicoRepositorio
    {
        private readonly IDbSession _session;

        public OrdemServicoRepositorio(IDbSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<IEnumerable<OrdemServico>> ObterTodosAsync(int pageSize, int page, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.OBTER_TODOS,
                new { PageSize = pageSize, Page = page },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<OrdemServico>(command).ConfigureAwait(false);
        }

        public async Task<IEnumerable<OrdemServico>> ObterOrdenadoPorStatusAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.OBTER_ORDENADO_POR_STATUS,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<OrdemServico>(command).ConfigureAwait(false);
        }

        public async Task<IEnumerable<OrdemServico>> ObterPorClienteIdAsync(int idCliente, int pageSize, int page, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.OBTER_POR_CLIENTE_ID,
                new { IdCliente = idCliente, PageSize = pageSize, Page = page },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<OrdemServico>(command).ConfigureAwait(false);
        }

        public async Task<decimal> ObterValorPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.OBTER_VALOR_POR_ID,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<decimal>(command).ConfigureAwait(false);
        }

        public async Task<OrdemServico?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.OBTER_POR_ID,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<OrdemServico>(command).ConfigureAwait(false);
        }

        public async Task<int> CriarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(ordemServico);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.CRIAR,
                ordemServico,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
        }

        public async Task<bool> AtualizarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(ordemServico);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.ATUALIZAR,
                ordemServico,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }

        public async Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                OrdemServicoQuerys.DELETAR,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }
    }
}
