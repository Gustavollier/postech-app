namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class OrdemServicoQuerys
    {
        public const string OBTER_TODOS = @"SELECT 
        Id, 
        IdCliente, 
        IdVeiculo, 
        IdFuncionario, 
        Status, 
        CriadoEm, 
        AtualizadoEm 
        FROM OrdemServico
        ORDER BY 
        Id, 
        IdCliente, 
        IdVeiculo, 
        IdFuncionario, 
        Status, 
        CriadoEm, 
        AtualizadoEm 
        OFFSET (@Page - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY";

        public const string OBTER_ORDENADO_POR_STATUS = @"
    SELECT
        Id,
        IdCliente,
        IdVeiculo,
        IdFuncionario,
        Status,
        CriadoEm,
        AtualizadoEm
    FROM OrdemServico
    WHERE Status NOT IN (4, 5)
    ORDER BY
        CASE
            WHEN Status = 3 THEN 1 -- EmExecucao
            WHEN Status = 2 THEN 2 -- AguardandoAprovacao
            WHEN Status = 1 THEN 3 -- EmDiagnostico
            WHEN Status = 0 THEN 4 -- Recebida
            ELSE 999
        END,
        CriadoEm ASC";

        public const string OBTER_VALOR_POR_ID = @"SELECT  
            COALESCE(SUM(
                CASE
                    WHEN IOS.TipoItem = 0 THEN ISNULL(FU.ValorHora, 0) * IOS.QuantidadeItem
                    WHEN IOS.TipoItem = 1 THEN ISNULL(PE.Preco, 0) * IOS.QuantidadeItem
                    ELSE 0
                END), 0) AS Valor
        FROM OrdemServico OS
        LEFT JOIN Itens IOS ON OS.Id = IOS.IdOs
        LEFT JOIN Pecas PE ON IOS.IdPeca = PE.Id
        LEFT JOIN Funcionario FU ON IOS.IdFuncionario = FU.Id
        WHERE OS.Id = @Id
        GROUP BY 
            OS.Id, 
            OS.IdCliente, 
            OS.IdVeiculo, 
            OS.IdFuncionario, 
            OS.Status, 
            OS.CriadoEm, 
            OS.AtualizadoEm";

        public const string OBTER_POR_ID = "SELECT Id, IdCliente, IdVeiculo, IdFuncionario, Status, CriadoEm, AtualizadoEm FROM OrdemServico WHERE Id = @Id";

        public const string OBTER_POR_CLIENTE_ID = @"SELECT 
        Id, 
        IdCliente, 
        IdVeiculo, 
        IdFuncionario, 
        Status, 
        CriadoEm, 
        AtualizadoEm 
        FROM OrdemServico
        WHERE IdCliente = @IdCliente
        ORDER BY 
        Id, 
        CriadoEm DESC
        OFFSET (@Page - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY";

        public const string CRIAR = @"
            INSERT INTO OrdemServico (IdCliente, IdVeiculo, IdFuncionario, Status, CriadoEm, AtualizadoEm)
            VALUES (@IdCliente, @IdVeiculo, @IdFuncionario, @Status, @CriadoEm, @AtualizadoEm);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE OrdemServico
            SET IdCliente = @IdCliente, IdVeiculo = @IdVeiculo, IdFuncionario = @IdFuncionario, Status = @Status, CriadoEm = @CriadoEm, AtualizadoEm = @AtualizadoEm
            WHERE Id = @Id";

        public const string DELETAR = @"
            DELETE FROM Orcamento WHERE IdOS = @Id;
            DELETE FROM Status WHERE IdOS = @Id;
            DELETE FROM Itens WHERE IdOS = @Id;
            DELETE FROM OrdemServico WHERE Id = @Id";
    }
}
