using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public class ItemsRepositorio : IItemsRepositorio
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ItemsRepositorio(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<ItemOS>> ObterTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryAsync<ItemOS>(QueryItens.OBTER_TODOS);
        }

        public async Task<ItemOS?> ObterPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<ItemOS>(QueryItens.OBTER_POR_ID, new { Id = id });
        }

        public async Task<int> CriarAsync(ItemOS item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.ExecuteScalarAsync<int>(QueryItens.CRIAR, item);
        }

        public async Task<bool> AtualizarAsync(ItemOS item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(QueryItens.ATUALIZAR, item);
            return result > 0;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(QueryItens.DELETAR, new { Id = id });
            return result > 0;
        }
    }
}
