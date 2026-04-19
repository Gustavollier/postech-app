namespace PosTechChallenge.Infraestrutura.Querys;

public static class SegurancaQuerys
{
    public const string OBTER_POR_FUNCIONARIO_ID = @"
        SELECT Id, FuncionarioId, SenhaHash, CriadoEm
        FROM dbo.Seguranca
        WHERE FuncionarioId = @FuncionarioId";

    public const string CRIAR = @"
        INSERT INTO dbo.Seguranca (FuncionarioId, SenhaHash, CriadoEm)
        VALUES (@FuncionarioId, @SenhaHash, @CriadoEm);
        SELECT CAST(SCOPE_IDENTITY() as int)";

    public const string ATUALIZAR = @"
        UPDATE dbo.Seguranca
        SET SenhaHash = @SenhaHash
        WHERE FuncionarioId = @FuncionarioId";
}
