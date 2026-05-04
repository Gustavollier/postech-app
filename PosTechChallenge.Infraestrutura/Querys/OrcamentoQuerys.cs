namespace PosTechChallenge.Infraestrutura.Querys;

public static class OrcamentoQuerys
{
    public const string OBTER_POR_ORDEM_SERVICO_ID = @"
        SELECT Id, IdOS, ValorMaoDeObra, ValorPecas, ValorTotal, Status, CriadoEm, AtualizadoEm
        FROM Orcamento
        WHERE IdOS = @IdOS";

    public const string CALCULAR_VALORES = @"
        SELECT
            COALESCE(SUM(CASE WHEN IOS.TipoItem = 0 THEN ISNULL(FU.ValorHora, 0) * IOS.QuantidadeItem ELSE 0 END), 0) AS ValorMaoDeObra,
            COALESCE(SUM(CASE WHEN IOS.TipoItem = 1 THEN ISNULL(PE.Preco, 0) * IOS.QuantidadeItem ELSE 0 END), 0) AS ValorPecas,
            COALESCE(SUM(CASE WHEN IOS.TipoItem = 0 THEN ISNULL(FU.ValorHora, 0) * IOS.QuantidadeItem ELSE 0 END), 0) +
            COALESCE(SUM(CASE WHEN IOS.TipoItem = 1 THEN ISNULL(PE.Preco, 0) * IOS.QuantidadeItem ELSE 0 END), 0) AS ValorTotal
        FROM OrdemServico OS
        LEFT JOIN Itens IOS ON OS.Id = IOS.IdOS
        LEFT JOIN Pecas PE ON IOS.IdPeca = PE.Id
        LEFT JOIN Funcionario FU ON IOS.IdFuncionario = FU.Id
        WHERE OS.Id = @IdOS";

    public const string CRIAR = @"
        INSERT INTO Orcamento (IdOS, ValorMaoDeObra, ValorPecas, ValorTotal, Status, CriadoEm, AtualizadoEm)
        VALUES (@IdOS, @ValorMaoDeObra, @ValorPecas, @ValorTotal, @Status, @CriadoEm, @AtualizadoEm);
        SELECT CAST(SCOPE_IDENTITY() as int);";

    public const string ATUALIZAR = @"
        UPDATE Orcamento
        SET ValorMaoDeObra = @ValorMaoDeObra,
            ValorPecas = @ValorPecas,
            ValorTotal = @ValorTotal,
            Status = @Status,
            AtualizadoEm = @AtualizadoEm
        WHERE Id = @Id";
}
