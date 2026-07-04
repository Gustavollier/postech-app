using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios;

public sealed class ClienteRepositorio : IClienteRepositorio
{
    private readonly IDbSession _session;

    public ClienteRepositorio(IDbSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<IEnumerable<Cliente>> ObterTodosAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            ClienteQuerys.OBTER_TODOS,
            new { Page = page, PageSize = pageSize },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<Cliente>(command).ConfigureAwait(false);
    }

    public async Task<int> ObterQuantidadeClientesAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            ClienteQuerys.OBTER_QUANTIDADE_CLIENTES,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command).ConfigureAwait(false);
    }

    public async Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            ClienteQuerys.OBTER_POR_ID,
            new { Id = id },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<Cliente>(command).ConfigureAwait(false);
    }

    public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            ClienteQuerys.OBTER_POR_CPF_CNPJ,
            new { CpfCnpj = cpfCnpj },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<Cliente>(command).ConfigureAwait(false);
    }

    public async Task<int> CriarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cliente);

        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            ClienteQuerys.CRIAR,
            cliente,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        // Retorna o Id gerado (SCOPE_IDENTITY).
        return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
    }

    public async Task<bool> AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cliente);

        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            ClienteQuerys.ATUALIZAR,
            cliente,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
        return linhasAfetadas > 0;
    }

    public async Task<bool> DesativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            ClienteQuerys.DESATIVAR,
            new { Id = id, UpdatedAt = DateTime.UtcNow },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
        return linhasAfetadas > 0;
    }
}
