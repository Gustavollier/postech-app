namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class QueryItens
    {
        public const string OBTER_TODOS = "SELECT Id, IdOS, TipoItem, QuantidadeItem, ISNULL(IdFuncionario, 0) AS IdFuncionario, ISNULL(IdPeca, 0) AS IdPeca FROM Itens";

        public const string OBTER_POR_ORDEM_SERVICO_ID = "SELECT Id, IdOS, TipoItem, QuantidadeItem, ISNULL(IdFuncionario, 0) AS IdFuncionario, ISNULL(IdPeca, 0) AS IdPeca FROM Itens WHERE IdOS = @IdOS";

        public const string OBTER_POR_ID = "SELECT Id, IdOS, TipoItem, QuantidadeItem, ISNULL(IdFuncionario, 0) AS IdFuncionario, ISNULL(IdPeca, 0) AS IdPeca FROM Itens WHERE Id = @Id";

        public const string CRIAR = @"
            INSERT INTO Itens (IdOS, TipoItem, QuantidadeItem, IdFuncionario, IdPeca)
            VALUES (@IdOS, @TipoItem, @QuantidadeItem, @IdFuncionario, @IdPeca);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Itens
            SET IdOS = @IdOS, TipoItem = @TipoItem, QuantidadeItem = @QuantidadeItem, IdFuncionario = @IdFuncionario, IdPeca = @IdPeca
            WHERE Id = @Id";

        public const string DELETAR = "DELETE FROM Itens WHERE Id = @Id";
    }
}
