using Dapper;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;
using PosTechChallenge.Dominio.Interfaces.Repositorios;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public class VeiculosRepositorio : IVeiculosRepositorio
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public VeiculosRepositorio(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            return await connection.QueryAsync<Veiculo>(VeiculoQuerys.OBTER_TODOS);
        }

        public async Task<Veiculo?> ObterPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<Veiculo>(VeiculoQuerys.OBTER_POR_ID, new { Id = id});
        }

        public async Task<int> CriarAsync(Veiculo veiculo)
        {
            if (veiculo == null)
                throw new ArgumentNullException(nameof(veiculo));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            return await connection.ExecuteScalarAsync<int>(VeiculoQuerys.CRIAR, veiculo);
        }

        public async Task<bool> AtualizarAsync(Veiculo veiculo)
        {
            if (veiculo == null)
                throw new ArgumentNullException(nameof(veiculo));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var result = await connection.ExecuteAsync(VeiculoQuerys.ATUALIZAR, veiculo);
            return result > 0;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var result = await connection.ExecuteAsync(VeiculoQuerys.DELETAR, new { Id = id });
            return result > 0;
        }
    }
}
