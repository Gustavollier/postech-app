using Moq;
using PosTechChallenge.Infraestrutura.Data;
using Xunit;

namespace PosTechChallenge.Testes.Infrastructure;

public class UnitOfWorkTests
{
    [Fact]
    public async Task Begin_Commit_Rollback_DeveDelegarParaSessionComOToken()
    {
        var session = new Mock<IDbSession>();
        var unitOfWork = new UnitOfWork(session.Object);
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        await unitOfWork.BeginTransactionAsync(token);
        await unitOfWork.CommitAsync(token);
        await unitOfWork.RollbackAsync(token);

        session.Verify(s => s.BeginTransactionAsync(token), Times.Once);
        session.Verify(s => s.CommitAsync(token), Times.Once);
        session.Verify(s => s.RollbackAsync(token), Times.Once);
    }

    [Fact]
    public void Construtor_SessionNula_DeveLancarArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(null!));
    }
}
