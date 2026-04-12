using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public class StatusRepositorio : IStatusRepositorio
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StatusRepositorio(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<Status>> ObterTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryAsync<Status>(StatusQuerys.OBTER_TODOS);
        }

        public async Task<Status?> ObterPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Status>(StatusQuerys.OBTER_POR_ID, new { Id = id });
        }

        public async Task<int> CriarAsync(Status status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.ExecuteScalarAsync<int>(StatusQuerys.CRIAR, status);
        }

        public async Task<bool> AtualizarAsync(Status status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(StatusQuerys.ATUALIZAR, status);
            return result > 0;
        }
    }
}
