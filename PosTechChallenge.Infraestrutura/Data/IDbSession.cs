using System.Data.Common;

namespace PosTechChallenge.Infraestrutura.Data;

/// <summary>
/// Sessão de banco compartilhada por escopo de requisição. É dona da única
/// <see cref="DbConnection"/> do escopo (aberta de forma assíncrona e reutilizada)
/// e da <see cref="DbTransaction"/> corrente. Os repositórios dependem desta
/// abstração para obter a conexão e a transação correntes.
/// </summary>
public interface IDbSession : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Retorna a conexão do escopo, abrindo-a de forma assíncrona na primeira chamada.
    /// Chamadas subsequentes reutilizam a mesma conexão aberta.
    /// </summary>
    Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>Transação corrente, ou <c>null</c> quando não há transação aberta.</summary>
    DbTransaction? Transaction { get; }

    /// <summary>Inicia uma transação sobre a conexão do escopo.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Efetiva (commit) e descarta a transação corrente.</summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>Desfaz (rollback) e descarta a transação corrente. Idempotente.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
