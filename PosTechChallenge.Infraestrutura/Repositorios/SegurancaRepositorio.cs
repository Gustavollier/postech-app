using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public sealed class SegurancaRepositorio : ISegurancaRepositorio
    {
        private readonly IDbSession _session;

        public SegurancaRepositorio(IDbSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<Seguranca?> ObterPorFuncionarioIdAsync(int funcionarioId, CancellationToken cancellationToken = default)
        {
            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(
                SegurancaQuerys.OBTER_POR_FUNCIONARIO_ID,
                new { FuncionarioId = funcionarioId },
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Seguranca>(command).ConfigureAwait(false);
        }

        public async Task SalvarSenhaAsync(int funcionarioId, string senhaHash, CancellationToken cancellationToken = default)
        {
            var segurancaExistente = await ObterPorFuncionarioIdAsync(funcionarioId, cancellationToken).ConfigureAwait(false);

            var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            var parametros = new
            {
                FuncionarioId = funcionarioId,
                SenhaHash = senhaHash,
                CriadoEm = DateTime.UtcNow
            };

            var sql = segurancaExistente is null ? SegurancaQuerys.CRIAR : SegurancaQuerys.ATUALIZAR;
            var command = new CommandDefinition(
                sql,
                parametros,
                transaction: _session.Transaction,
                cancellationToken: cancellationToken);

            await connection.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
