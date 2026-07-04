using PosTechChallenge.Infraestrutura.Data;
using Xunit;

namespace PosTechChallenge.Testes.Infrastructure;

public class DbSessionTests
{
    private const string ConnectionString =
        "Server=localhost;Database=Fake;User Id=sa;Password=fake;TrustServerCertificate=True;";

    [Fact]
    public void Construtor_ConnectionStringNula_DeveLancarArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DbSession(null!));
    }

    [Fact]
    public async Task CommitAsync_SemTransacao_DeveLancarInvalidOperation()
    {
        await using var session = new DbSession(ConnectionString);

        // Sem transação aberta, o commit falha sem sequer tocar o banco.
        await Assert.ThrowsAsync<InvalidOperationException>(() => session.CommitAsync());
    }

    [Fact]
    public async Task RollbackAsync_SemTransacao_DeveSerNoOp()
    {
        await using var session = new DbSession(ConnectionString);

        await session.RollbackAsync();

        Assert.Null(session.Transaction);
    }

    [Fact]
    public async Task Dispose_DeveSerIdempotente()
    {
        var session = new DbSession(ConnectionString);

        await session.DisposeAsync();
        await session.DisposeAsync();
        session.Dispose();

        Assert.Null(session.Transaction);
    }
}
