namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class FuncionarioQuerys
    {
        public const string OBTER_TODOS = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionario";

        public const string OBTER_POR_ID = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionario WHERE Id = @Id";

        public const string OBTER_POR_CPF = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionario WHERE CPF = @CPF";

        public const string CRIAR = @"
            INSERT INTO Funcionario (Nome, Contato, CPF, Cargo, ValorHora)
            VALUES (@Nome, @Contato, @CPF, @Cargo, @ValorHora);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Funcionario
            SET Nome = @Nome, Contato = @Contato, CPF = @CPF, Cargo = @Cargo, ValorHora = @ValorHora
            WHERE Id = @Id";

        public const string DELETAR = "DELETE FROM Funcionario WHERE Id = @Id";
    }
}
