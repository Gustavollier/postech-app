namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class StatusQuerys
    {
        public const string OBTER_TODOS = "SELECT Id, IdOS, UpdatedAt, IdFuncionario, StatusAtual FROM Status";

        public const string OBTER_POR_ORDEM_SERVICO_ID = "SELECT Id, IdOS, UpdatedAt, IdFuncionario, StatusAtual FROM Status WHERE IdOS = @IdOS ORDER BY UpdatedAt";

        public const string OBTER_POR_ID = "SELECT Id, IdOS, UpdatedAt, IdFuncionario, StatusAtual FROM Status WHERE Id = @Id";

        public const string CRIAR = @"
            INSERT INTO Status (IdOS, UpdatedAt, IdFuncionario, StatusAtual)
            VALUES (@IdOS, @UpdatedAt, @IdFuncionario, @StatusAtual);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Status
            SET IdOS = @IdOS, UpdatedAt = @UpdatedAt, IdFuncionario = @IdFuncionario, StatusAtual = @StatusAtual
            WHERE Id = @Id";
    }
}
