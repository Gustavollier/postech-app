using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositories;
public class FuncionariosRepositorio : IFuncionarioRepositorio
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FuncionariosRepositorio(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IEnumerable<Funcionario>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.QueryAsync<Funcionario>(FuncionarioQuerys.OBTER_TODOS);
    }

    public async Task<Funcionario?> ObterPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<Funcionario>(FuncionarioQuerys.OBTER_POR_ID, new { Id = id });
    }

    public async Task<Funcionario?> ObterPorCPFAsync(string CPF)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<Funcionario>(FuncionarioQuerys.OBTER_POR_CPF, new { CPF = CPF });
    }

    public async Task<int> CriarAsync(Funcionario funcionario)
    {
        if (funcionario == null)
            throw new ArgumentNullException(nameof(funcionario));

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.ExecuteScalarAsync<int>(FuncionarioQuerys.CRIAR, funcionario);
    }

    public async Task<bool> AtualizarAsync(Funcionario funcionario)
    {
        if (funcionario == null)
            throw new ArgumentNullException(nameof(funcionario));

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteAsync(FuncionarioQuerys.ATUALIZAR, funcionario);
        return result > 0;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteAsync(FuncionarioQuerys.DELETAR, new { Id = id });
        return result > 0;
    }
}
