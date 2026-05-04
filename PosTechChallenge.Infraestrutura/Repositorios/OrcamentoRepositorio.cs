using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios;

public sealed class OrcamentoRepositorio : IOrcamentoRepositorio
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OrcamentoRepositorio(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Orcamento?> ObterPorOrdemServicoIdAsync(int ordemServicoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<Orcamento>(
            OrcamentoQuerys.OBTER_POR_ORDEM_SERVICO_ID,
            new { IdOS = ordemServicoId });
    }

    public async Task<OrcamentoValores> CalcularValoresAsync(int ordemServicoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return await connection.QuerySingleAsync<OrcamentoValores>(
            OrcamentoQuerys.CALCULAR_VALORES,
            new { IdOS = ordemServicoId });
    }

    public async Task<int> CriarAsync(Orcamento orcamento)
    {
        if (orcamento == null)
            throw new ArgumentNullException(nameof(orcamento));

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return await connection.ExecuteScalarAsync<int>(OrcamentoQuerys.CRIAR, orcamento);
    }

    public async Task<bool> AtualizarAsync(Orcamento orcamento)
    {
        if (orcamento == null)
            throw new ArgumentNullException(nameof(orcamento));

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var result = await connection.ExecuteAsync(OrcamentoQuerys.ATUALIZAR, orcamento);
        return result > 0;
    }
}
