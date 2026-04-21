using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios;

public sealed class ClienteRepositorio : IClienteRepositorio
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClienteRepositorio(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IEnumerable<Cliente>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.QueryAsync<Cliente>(ClienteQuerys.OBTER_TODOS);
    }

    public async Task<Cliente?> ObterPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<Cliente>(ClienteQuerys.OBTER_POR_ID, new { Id = id });
    }

    public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<Cliente>(ClienteQuerys.OBTER_POR_CPF_CNPJ, new { CpfCnpj = cpfCnpj });
    }

    public async Task<int> CriarAsync(Cliente cliente)
    {
        if (cliente == null)
            throw new ArgumentNullException(nameof(cliente));

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        return await connection.ExecuteScalarAsync<int>(ClienteQuerys.CRIAR, cliente);
    }

    public async Task<bool> AtualizarAsync(Cliente cliente)
    {
        if (cliente == null)
            throw new ArgumentNullException(nameof(cliente));

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteAsync(ClienteQuerys.ATUALIZAR, cliente);
        return result > 0;
    }

    public async Task<bool> DesativarAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteAsync(ClienteQuerys.DESATIVAR, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return result > 0;
    }
}