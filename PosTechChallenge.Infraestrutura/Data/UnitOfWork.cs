using PosTechChallenge.Dominio.Interfaces;

namespace PosTechChallenge.Infraestrutura.Data;

/// <summary>
/// Implementação de <see cref="IUnitOfWork"/> que delega o controle transacional
/// à <see cref="IDbSession"/> compartilhada do escopo. Mantém o domínio livre de
/// detalhes de infraestrutura: o UseCase só conhece <see cref="IUnitOfWork"/>.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbSession _session;

    public UnitOfWork(IDbSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _session.BeginTransactionAsync(cancellationToken);

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => _session.CommitAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default)
        => _session.RollbackAsync(cancellationToken);
}
