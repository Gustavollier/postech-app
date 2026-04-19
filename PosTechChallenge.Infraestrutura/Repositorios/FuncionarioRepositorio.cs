using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PosTechChallenge.Infraestrutura.Repositorios
{
    public class FuncionarioRepositorio : IFuncionarioRepositorio
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FuncionarioRepositorio(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Funcionario?> ObterPorCPFAsync(string cpf)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Funcionario>(
                FuncionarioQuerys.OBTER_POR_CPF,
                new { CPF = cpf });
        }

        public Task<IEnumerable<Funcionario>> ObterTodosAsync() => throw new NotImplementedException();
        public Task<Funcionario?> ObterPorIdAsync(int id) => throw new NotImplementedException();
        public Task<int> CriarAsync(Funcionario funcionario) => throw new NotImplementedException();
        public Task<bool> AtualizarAsync(Funcionario funcionario) => throw new NotImplementedException();
        public Task<bool> DeletarAsync(int id) => throw new NotImplementedException();
    }
}
