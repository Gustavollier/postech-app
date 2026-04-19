using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositories;

public class SegurancaRepositorio : ISegurancaFuncionarioRepositorio
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SegurancaRepositorio(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<SegurancaFuncionario?> ObterPorFuncionarioIdAsync(int funcionarioId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<SegurancaFuncionario>(
            SegurancaQuerys.OBTER_POR_FUNCIONARIO_ID, 
            new { FuncionarioId = funcionarioId });
    }

    public async Task<int> CriarAsync(SegurancaFuncionario seguranca)
    {
        if (seguranca == null) throw new ArgumentNullException(nameof(seguranca));
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return await connection.ExecuteScalarAsync<int>(SegurancaQuerys.CRIAR, seguranca);
    }

    public async Task<bool> AtualizarAsync(SegurancaFuncionario seguranca)
    {
        if (seguranca == null) throw new ArgumentNullException(nameof(seguranca));
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var result = await connection.ExecuteAsync(SegurancaQuerys.ATUALIZAR, seguranca);
        return result > 0;
    }
}