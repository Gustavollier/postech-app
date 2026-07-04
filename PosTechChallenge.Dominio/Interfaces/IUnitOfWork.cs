namespace PosTechChallenge.Dominio.Interfaces;

/// <summary>
/// Unidade de trabalho: controla explicitamente o ciclo de vida de uma transação
/// que agrupa múltiplas operações de escrita (insert/update/delete) de forma atômica.
/// A abstração é intencionalmente livre de detalhes de infraestrutura (não expõe
/// <c>IDbConnection</c>/<c>IDbTransaction</c>) para manter o domínio limpo.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Inicia uma nova transação no escopo atual.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Efetiva (commit) a transação corrente.</summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>Desfaz (rollback) a transação corrente. Idempotente: não faz nada se não houver transação.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
