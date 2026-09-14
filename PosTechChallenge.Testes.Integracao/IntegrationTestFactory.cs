using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace PosTechChallenge.Testes.Integracao;

/// <summary>
/// Sobe o host real da API (sem mockar serviços) apontando para um SQL Server
/// provisionado via docker-compose, exercitando os repositórios e o acesso a dados
/// contra um banco real.
///
/// A connection string vem da variável de ambiente <c>INTEGRATION_TEST_DB_CONNECTION</c>
/// (definida na pipeline). Localmente, cai no default apontando para localhost:1433.
/// </summary>
public sealed class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string JwtSecret = "estaEhUmaChaveSuper-SeguraComMaisDeTrintaCaracteresAlatorios!@#$%^&*()";

    public string ConnectionString { get; }

    public IntegrationTestFactory()
    {
        ConnectionString = Environment.GetEnvironmentVariable("INTEGRATION_TEST_DB_CONNECTION")
            ?? "Server=localhost,1433;Database=PosTechChallenge;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False;";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
        builder.UseSetting("Jwt:SecretKey", JwtSecret);
        builder.UseSetting("Jwt:Issuer", "PosTechChallenge");
        builder.UseSetting("Jwt:Audience", "PosTechChallenge-API");
        builder.UseSetting("Jwt:AccessTokenExpirationMinutes", "15");
        builder.UseSetting("HttpsRedirection:Enabled", "false");
        builder.UseEnvironment("Development");
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await AguardarBancoDisponivelAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Remove apenas os dados criados pelos testes (identificados pelo CPF), garantindo
    /// isolamento sem afetar os registros de seed referenciados por Veiculo/OrdemServico.
    /// </summary>
    public async Task LimparBancoAsync(string cpf)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        // Ordem respeita a FK Seguranca -> Funcionario.
        await connection.ExecuteAsync(
            """
            DELETE s FROM Seguranca s
            INNER JOIN Funcionario f ON f.Id = s.FuncionarioId
            WHERE f.CPF = @Cpf;

            DELETE FROM Funcionario WHERE CPF = @Cpf;
            DELETE FROM Cliente WHERE CPF = @Cpf;
            """,
            new { Cpf = cpf });
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        return await connection.QuerySingleOrDefaultAsync<T>(sql, param);
    }

    public static string GerarToken(string cargo = "Gerente", int funcionarioId = 1)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, funcionarioId.ToString()),
            new Claim(ClaimTypes.Role, cargo),
        };

        var token = new JwtSecurityToken(
            issuer: "PosTechChallenge",
            audience: "PosTechChallenge-API",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Token equivalente ao que a funcao de autenticacao por CPF emite: papel
    /// "Cliente" e o id do cliente em "ClienteId". E com ele que se verifica
    /// que um cliente nao alcanca dado de outro.
    /// </summary>
    public static string GerarTokenCliente(int clienteId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, clienteId.ToString()),
            new Claim(ClaimTypes.Role, "Cliente"),
            new Claim("ClienteId", clienteId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "PosTechChallenge",
            audience: "PosTechChallenge-API",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task AguardarBancoDisponivelAsync(int tentativas = 30, int intervaloMs = 2000)
    {
        for (var i = 1; i <= tentativas; i++)
        {
            try
            {
                await using var connection = new SqlConnection(ConnectionString);
                await connection.OpenAsync();
                await connection.ExecuteAsync("SELECT 1");
                return;
            }
            catch (SqlException) when (i < tentativas)
            {
                await Task.Delay(intervaloMs);
            }
        }

        throw new InvalidOperationException(
            $"SQL Server não ficou disponível na connection string configurada após {tentativas} tentativas.");
    }
}
