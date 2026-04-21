namespace PosTechChallenge.Infraestrutura.Querys;

public static class ClienteQuerys
{
    public const string OBTER_TODOS = @"
        SELECT Id, CreatedAt, UpdatedAt, CPF, CNPJ, NomeCompleto, Telefone, Email, Ativo
        FROM Cliente
        WHERE Ativo = 1
        ORDER BY Id DESC";

    public const string OBTER_POR_ID = @"
        SELECT Id, CreatedAt, UpdatedAt, CPF, CNPJ, NomeCompleto, Telefone, Email, Ativo
        FROM Cliente
        WHERE Id = @Id AND Ativo = 1";

    public const string OBTER_POR_CPF_CNPJ = @"
        SELECT Id, CreatedAt, UpdatedAt, CPF, CNPJ, NomeCompleto, Telefone, Email, Ativo
        FROM Cliente
        WHERE Ativo = 1 AND (CPF = @CpfCnpj OR CNPJ = @CpfCnpj)";

    public const string CRIAR = @"
        INSERT INTO Cliente (CreatedAt, UpdatedAt, CPF, CNPJ, NomeCompleto, Telefone, Email, Ativo)
        VALUES (@CreatedAt, @UpdatedAt, @CPF, @CNPJ, @NomeCompleto, @Telefone, @Email, @Ativo);
        SELECT CAST(SCOPE_IDENTITY() as int);";

    public const string ATUALIZAR = @"
        UPDATE Cliente
        SET UpdatedAt = @UpdatedAt,
            CPF = @CPF,
            CNPJ = @CNPJ,
            NomeCompleto = @NomeCompleto,
            Telefone = @Telefone,
            Email = @Email,
            Ativo = @Ativo
        WHERE Id = @Id";

    public const string DESATIVAR = @"
        UPDATE Cliente
        SET UpdatedAt = @UpdatedAt,
            Ativo = 0
        WHERE Id = @Id";
}