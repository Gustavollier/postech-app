using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace PosTechChallenge.Infraestrutura.Data;

/// <summary>
/// Implementação de <see cref="IDbSession"/> baseada em <see cref="SqlConnection"/>.
/// Deve ser registrada como <c>Scoped</c> para que exista uma única conexão por
/// requisição. O disposal é determinístico: ao final do escopo o DI descarta a
/// sessão, que por sua vez descarta transação e conexão (evitando vazamentos).
/// </summary>
public sealed class DbSession : IDbSession
{
    private readonly string _connectionString;
    private DbConnection? _connection;
    private DbTransaction? _transaction;
    private bool _disposed;

    public DbSession(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public DbTransaction? Transaction => _transaction;

    public async Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _connection ??= new SqlConnection(_connectionString);

        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }

        return _connection;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_transaction is not null)
        {
            throw new InvalidOperationException("Já existe uma transação em andamento nesta sessão.");
        }

        var connection = await GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        _transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("Não há transação para efetivar (commit).");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await DisposeTransactionAsync().ConfigureAwait(false);
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        // Idempotente: se não há transação, não há o que desfazer.
        if (_transaction is null)
        {
            return;
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await DisposeTransactionAsync().ConfigureAwait(false);
        }
    }

    private async ValueTask DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        await DisposeTransactionAsync().ConfigureAwait(false);

        if (_connection is not null)
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
            _connection = null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _transaction?.Dispose();
        _transaction = null;
        _connection?.Dispose();
        _connection = null;
    }
}
