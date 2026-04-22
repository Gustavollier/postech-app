using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public class PecasRepositorio : IPecasRepositorio
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PecasRepositorio(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<Pecas>> ObterTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryAsync<Pecas>(PecasQuerys.OBTER_TODOS);
        }

        public async Task<Pecas?> ObterPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Pecas>(PecasQuerys.OBTER_POR_ID, new { Id = id });
        }

        public async Task<int> CriarAsync(Pecas peca)
        {
            if (peca == null)
                throw new ArgumentNullException(nameof(peca));
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.ExecuteScalarAsync<int>(PecasQuerys.CRIAR, peca);
        }

        public async Task<bool> AtualizarAsync(Pecas peca)
        {
            if (peca == null)
                throw new ArgumentNullException(nameof(peca));
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(PecasQuerys.ATUALIZAR, peca);
            return result > 0;
        }

        public async Task<bool> AjustarEstoqueAsync(int id, int quantidadeEstoque, DateTime atualizadoEm)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(PecasQuerys.AJUSTAR_ESTOQUE, new { Id = id, QuantidadeEstoque = quantidadeEstoque, AtualizadoEm = atualizadoEm });
            return result > 0;
        }

        public async Task<bool> DeletarAsync(int id, DateTime atualizadoEm)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var result = await connection.ExecuteAsync(PecasQuerys.DELETAR, new { Id = id, AtualizadoEm = atualizadoEm });
            return result > 0;
        }
    }
}
