using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios;

public sealed class EmailOutboxRepositorio : IEmailOutboxRepositorio
{
    private readonly IDbSession _session;

    public EmailOutboxRepositorio(IDbSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<int> CriarAsync(EmailOutbox emailOutbox, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emailOutbox);

        var connection = await _session.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(
            EmailOutboxQuerys.CRIAR,
            emailOutbox,
            transaction: _session.Transaction,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
    }
}
