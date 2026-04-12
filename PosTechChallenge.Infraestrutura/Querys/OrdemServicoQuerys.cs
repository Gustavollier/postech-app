namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class OrdemServicoQuerys
    {
        public const string OBTER_TODOS = "SELECT Id, IdCliente, IdVeiculo, IdFuncionario, Status, CriadoEm, AtualizadoEm FROM OrdemServico";

        public const string OBTER_POR_ID = "SELECT Id, IdCliente, IdVeiculo, IdFuncionario, Status, CriadoEm, AtualizadoEm FROM OrdemServico WHERE Id = @Id";

        public const string CRIAR = @"
            INSERT INTO OrdemServico (IdCliente, IdVeiculo, IdFuncionario, Status, CriadoEm, AtualizadoEm)
            VALUES (@IdCliente, @IdVeiculo, @IdFuncionario, @Status, @CriadoEm, @AtualizadoEm);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE OrdemServico
            SET IdCliente = @IdCliente, IdVeiculo = @IdVeiculo, IdFuncionario = @IdFuncionario, Status = @Status, CriadoEm = @CriadoEm, AtualizadoEm = @AtualizadoEm
            WHERE Id = @Id";

        public const string DELETAR = "DELETE FROM OrdemServico WHERE Id = @Id";
    }
}
