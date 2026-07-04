using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public sealed class VeiculosRepositorio : IVeiculosRepositorio
    {
        private readonly IDbSession _session;

        public VeiculosRepositorio(IDbSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                VeiculoQuerys.OBTER_TODOS,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Veiculo>(command).ConfigureAwait(false);
        }

        public async Task<Veiculo?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                VeiculoQuerys.OBTER_POR_ID,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Veiculo>(command).ConfigureAwait(false);
        }

        public async Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                VeiculoQuerys.OBTER_POR_PLACA,
                new { Placa = placa },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Veiculo>(command).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(int clienteId, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                VeiculoQuerys.OBTER_POR_CLIENTE_ID,
                new { ClienteId = clienteId },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Veiculo>(command).ConfigureAwait(false);
        }

        public async Task<int> CriarAsync(Veiculo veiculo, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(veiculo);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                VeiculoQuerys.CRIAR,
                veiculo,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
        }

        public async Task<bool> AtualizarAsync(Veiculo veiculo, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(veiculo);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                VeiculoQuerys.ATUALIZAR,
                veiculo,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }

        public async Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                VeiculoQuerys.DELETAR,
                new { Id = id },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
            return linhasAfetadas > 0;
        }
    }
}
