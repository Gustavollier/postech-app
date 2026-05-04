using Dapper;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Infraestrutura.Querys;

namespace PosTechChallenge.Infraestrutura.Repositorios;

public sealed class EmailOutboxRepositorio : IEmailOutboxRepositorio
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmailOutboxRepositorio(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<int> CriarAsync(EmailOutbox emailOutbox)
    {
        if (emailOutbox == null)
            throw new ArgumentNullException(nameof(emailOutbox));

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return await connection.ExecuteScalarAsync<int>(EmailOutboxQuerys.CRIAR, emailOutbox);
    }
}
