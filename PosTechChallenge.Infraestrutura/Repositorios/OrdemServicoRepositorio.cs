using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public class OrdemServicoRepositorio : IOrdemServicoRepositorio
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrdemServicoRepositorio(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<OrdemServico>> ObterTodosAsync(int pageSize, int page)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryAsync<OrdemServico>(OrdemServicoQuerys.OBTER_TODOS, new { PageSize = pageSize, Page = page });
        }

        public async Task<decimal> ObterValorPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QuerySingleAsync<decimal>(OrdemServicoQuerys.OBTER_VALOR_POR_ID, new { Id = id });
        }

        public async Task<OrdemServico?> ObterPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<OrdemServico>(OrdemServicoQuerys.OBTER_POR_ID, new { Id = id });
        }

        public async Task<int> CriarAsync(OrdemServico ordemServico)
        {
            if (ordemServico == null)
                throw new ArgumentNullException(nameof(ordemServico));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.ExecuteScalarAsync<int>(OrdemServicoQuerys.CRIAR, ordemServico);
        }

        public async Task<bool> AtualizarAsync(OrdemServico ordemServico)
        {
            if (ordemServico == null)
                throw new ArgumentNullException(nameof(ordemServico));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(OrdemServicoQuerys.ATUALIZAR, ordemServico);
            return result > 0;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(OrdemServicoQuerys.DELETAR, new { Id = id });
            return result > 0;
        }
    }
}
