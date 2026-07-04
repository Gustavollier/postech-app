using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios;

public sealed class FuncionariosRepositorio : IFuncionarioRepositorio
{
    private readonly IDbSession _session;

    public FuncionariosRepositorio(IDbSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<IEnumerable<Funcionario>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            FuncionarioQuerys.OBTER_TODOS,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<Funcionario>(command).ConfigureAwait(false);
    }

    public async Task<Funcionario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            FuncionarioQuerys.OBTER_POR_ID,
            new { Id = id },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<Funcionario>(command).ConfigureAwait(false);
    }

    public async Task<Funcionario?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            FuncionarioQuerys.OBTER_POR_NOME,
            new { Nome = nome },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<Funcionario>(command).ConfigureAwait(false);
    }

    public async Task<Funcionario?> ObterPorCPFAsync(string CPF, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            FuncionarioQuerys.OBTER_POR_CPF,
            new { CPF = CPF },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<Funcionario>(command).ConfigureAwait(false);
    }

    public async Task<int> CriarAsync(Funcionario funcionario, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(funcionario);

        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            FuncionarioQuerys.CRIAR,
            funcionario,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        // Retorna o Id gerado (SCOPE_IDENTITY).
        return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
    }

    public async Task<bool> AtualizarAsync(Funcionario funcionario, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(funcionario);

        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            FuncionarioQuerys.ATUALIZAR,
            funcionario,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
        return linhasAfetadas > 0;
    }

    public async Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            FuncionarioQuerys.DELETAR,
            new { Id = id },
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        var linhasAfetadas = await connection.ExecuteAsync(command).ConfigureAwait(false);
        return linhasAfetadas > 0;
    }
}
