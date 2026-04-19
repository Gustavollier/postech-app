using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;
using System.Data;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public class SegurancaRepositorio : ISegurancaRepositorio
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SegurancaRepositorio(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Seguranca?> ObterPorFuncionarioIdAsync(int funcionarioId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Seguranca>(
                SegurancaQuerys.OBTER_POR_FUNCIONARIO_ID,
                new { FuncionarioId = funcionarioId });
        }

        public async Task CriarSenhaAsync(int funcionarioId, string senhaHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var seguranca = new
            {
                FuncionarioId = funcionarioId,
                SenhaHash = senhaHash,
                CriadoEm = DateTime.UtcNow
            };
            await connection.ExecuteScalarAsync<int>(SegurancaQuerys.CRIAR, seguranca);
        }
    }
}